using DotBpm.Sdk.Commands;
using DotBpm.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotBpm.Bpmn.BpmnModel;
using System.Diagnostics;
using DotBpm.ServiceTask;
using System.ComponentModel;
using DotBpm.StorageEngine;

namespace Engines
{
    public class RunEngine
    {
        private ProcessInstance processInstance;
        private IExecutionScopeStore executionScopeStore;
        private IElementLogStore elementLogStore;

        private BlockingCollection<Command> commands = new BlockingCollection<Command>(new ConcurrentQueue<Command>());

        public RunEngine(ProcessInstance processInstance, IExecutionScopeStore executionScopeStore, IElementLogStore elementLogStore)
        {
            this.processInstance = processInstance;
            this.executionScopeStore = executionScopeStore;
            this.elementLogStore = elementLogStore;
        }

        public async Task ExecuteProcess()
        {
            await ExecuteProcess(new Dictionary<string, object>());
        }

        public async Task ExecuteProcess(Dictionary<string, object> variables)
        {
            InitializeProcessInstance(variables);
            SetStartCommands();

            await Task.Run(() =>
            {
                foreach (var command in commands.GetConsumingEnumerable())
                {
                    ExecuteBackgroundWorker(command);
                }
            });
        }

        private void InitializeProcessInstance(Dictionary<string, object> variables)
        {
            processInstance.ExecutionScope = executionScopeStore.Create(processInstance.Id, processInstance.BpmnProcess.Id);
            processInstance.ExecutionScope.Variables = variables;
        }

        private void SetStartCommands()
        {
            StartProcess(processInstance.BpmnProcess.Artifacts.OfType<BpmnStartEvent>());
        }

        private void StartProcess(IEnumerable<BpmnStartEvent> startEvents)
        {
            foreach (var startEvent in startEvents)
            {
                var token = new ProcessToken(startEvent.Id);
                processInstance.Tokens.Add(token.Id, token);
                commands.Add(new ExecuteTokenCommand() { Token = token });
            }
        }

        private void ExecuteBackgroundWorker(Command command)
        {
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += Bgw_DoWork;
            bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            bgw.RunWorkerAsync(command);
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (processInstance.Tokens)
            {
                if (processInstance.Tokens.Count(t => t.Value.Status == TokenStatus.Inactive) == processInstance.Tokens.Count)
                {
                    commands.CompleteAdding();
                }
            }
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            ExecuteCommand((Command)e.Argument);
        }

        private void ExecuteCommand(Command command)
        {
            if(command is ProceedTokenCommand)
            {
                HandleProceedTokenCommand((ProceedTokenCommand)command);
            }
            else if(command is ExecuteTokenCommand)
            {
                HandleExecuteTokenCommand((ExecuteTokenCommand)command);
            }
        }
        
        private void HandleProceedTokenCommand(ProceedTokenCommand command)
        {
            lock (processInstance.Tokens)
            {
                var currentBpmnElement = processInstance.BpmnProcess.Artifacts.First(t => t.Id == command.Token.CurrentElementId);
                if (currentBpmnElement is BpmnSequenceFlow)
                {
                    // Next must be an Element
                    Trace.WriteLine("Token on : " + command.Token.CurrentElementId);
                    
                    var sequenceFlow = (BpmnSequenceFlow)currentBpmnElement;
                    HandleSequenceConditionExpression(command, sequenceFlow.ConditionExpression);

                    // The new token must take the status of the previous token over, so that inactive token proceed
                    // further as inactive tokens
                    var token = new ProcessToken(sequenceFlow.TargetRef, command.Token.Status);
                    processInstance.Tokens.Add(token.Id, token);
                    commands.Add(new ExecuteTokenCommand() { Token = token });
                }
                else if (currentBpmnElement is BpmnFlowNode)
                {
                    // Create token for each outgoing and deactivate current token
                    var flowNode = (BpmnFlowNode)currentBpmnElement;

                    foreach (var outgoing in flowNode.Outgoing)
                    {
                        // The new token must take the status of the previous token over, so that inactive token proceed
                        // further as inactive tokens
                        var token = new ProcessToken(outgoing, command.Token.Status);
                        processInstance.Tokens.Add(token.Id, token);
                        commands.Add(new ProceedTokenCommand() { Token = token });
                    }
                }
                else
                {
                    throw new Exception("Unexpected BpmnElement on ExecuteCommand " + currentBpmnElement.GetType().Name);
                }

                processInstance.Tokens[command.Token.Id].Status = TokenStatus.Inactive;
            }
        }

        private void HandleSequenceConditionExpression(ProceedTokenCommand command, BpmnFormalExpression conditionExpression)
        {
            if(string.IsNullOrEmpty(conditionExpression.Language) || string.IsNullOrEmpty(conditionExpression.Body))
            {
                return;
            }

            string script = "function ExecuteConditionExpression() {\n" + conditionExpression.Body + "\n}\nExecuteConditionExpression();";
            foreach (var variable in processInstance.ExecutionScope.Variables)
            {
                script = script.Replace("${" + variable.Key + "}", variable.Value.ToString());
            }

            var engine = new Jint.Engine();
            var result = engine.Execute(script)
                .GetCompletionValue() // get the latest statement completion value
                .ToObject(); // converts the value to .NET

            if(!(bool)result)
            {
                command.Token.Status = TokenStatus.Inactive;
            }
        }

        private void HandleExecuteTokenCommand(ExecuteTokenCommand command)
        {
            Trace.WriteLine("Token on : " + command.Token.CurrentElementId + " with status " + command.Token.Status);
            
            var currentBpmnElement = processInstance.BpmnProcess.ArtifactIndex[command.Token.CurrentElementId];
            if(currentBpmnElement is BpmnServiceTask && command.Token.Status == TokenStatus.Active)
            {
                processInstance.Tokens[command.Token.Id].Status = TokenStatus.InExecution;

                var taskExecutionScope = executionScopeStore.Create(processInstance.Id, currentBpmnElement.Id, currentBpmnElement.ParentBpmnElement.Id);

                var sleepTask = new SleepTask();
                sleepTask.Execute(new ServiceTaskContext(command.Token, taskExecutionScope));
            }

            if(currentBpmnElement is BpmnSubProcess)
            {
                StartProcess(((BpmnSubProcess)currentBpmnElement).Artifacts.OfType<BpmnStartEvent>());
            }

            if(currentBpmnElement is BpmnEndEvent)
            {
                var bpmnEndEvent = (BpmnEndEvent)currentBpmnElement;
                if(bpmnEndEvent.ParentBpmnElement is BpmnSubProcess)
                {
                    var subProcess = (BpmnSubProcess)bpmnEndEvent.ParentBpmnElement;
                    
                    // TODO Find out if all endevents of a process have been finished
                    // If yes, the the subprocess has ended and must move its token
                }
            }

            if (currentBpmnElement is BpmnGateway)
            {
                var gateway = (BpmnGateway)currentBpmnElement;
                int numberOfTokensReceivedOnGateway = processInstance.Tokens.Count(t => t.Value.CurrentElementId == command.Token.CurrentElementId);
                if(gateway.Incoming.Count == numberOfTokensReceivedOnGateway)
                {
                    commands.Add(new ProceedTokenCommand() { Token = command.Token });
                }
                else
                {
                    // If the token is not forwarded, it must be deactivated
                    processInstance.Tokens[command.Token.Id].Status = TokenStatus.Inactive;
                }
            }
            else
            {
                commands.Add(new ProceedTokenCommand() { Token = command.Token });
            }
        }
    }
}
