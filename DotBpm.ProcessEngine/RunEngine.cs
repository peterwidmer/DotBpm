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
using DotBpm.Engines;
using System.Text.RegularExpressions;

namespace DotBpm.Engines
{
    public class RunEngine
    {
        private ProcessInstance processInstance;
        private IExecutionScopeStore executionScopeStore;
        private IServiceTaskEngine serviceTaskEngine;

        private BlockingCollection<Command> commands = new BlockingCollection<Command>(new ConcurrentQueue<Command>());

        public RunEngine(ProcessInstance processInstance, IExecutionScopeStore executionScopeStore, IServiceTaskEngine serviceTaskEngine)
        {
            this.processInstance = processInstance;
            this.executionScopeStore = executionScopeStore;
            this.serviceTaskEngine = serviceTaskEngine;
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
                var token = new ProcessToken(startEvent);
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
                var currentBpmnElement = processInstance.BpmnProcess.ArtifactIndex[command.Token.CurrentElement.Id];
                if (currentBpmnElement is BpmnSequenceFlow)
                {
                    // Next must be an Element
                    Trace.WriteLine("Token on : " + command.Token.CurrentElement.Id);

                    var sequenceFlow = (BpmnSequenceFlow)currentBpmnElement;
                    HandleSequenceConditionExpression(command, sequenceFlow.ConditionExpression);

                    // The new token must take the status of the previous token over, so that inactive token proceed
                    // further as inactive tokens
                    var token = new ProcessToken(processInstance.BpmnProcess.ArtifactIndex[sequenceFlow.TargetRef], command.Token.Status);
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
                        var token = new ProcessToken(processInstance.BpmnProcess.ArtifactIndex[outgoing], command.Token.Status);
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
            Trace.WriteLine("Token on : " + command.Token.CurrentElement.Id + " with status " + command.Token.Status);
            
            var currentBpmnElement = processInstance.BpmnProcess.ArtifactIndex[command.Token.CurrentElement.Id];
            if(currentBpmnElement is BpmnServiceTask && command.Token.Status == TokenStatus.Active)
            {
                var serviceTaskBpmnElement = (BpmnServiceTask)currentBpmnElement;

                processInstance.Tokens[command.Token.Id].Status = TokenStatus.InExecution;

                var serviceTaskExecutionScope = executionScopeStore.Create(processInstance.Id, currentBpmnElement.Id, currentBpmnElement.ParentBpmnElement.Id);
                
                var serviceTask = serviceTaskEngine.GetInstance(serviceTaskBpmnElement.Class);
                var serviceTaskContext = new ServiceTaskContext(command.Token, serviceTaskExecutionScope);

                foreach (var inputParameter in serviceTaskBpmnElement.InputParameters)
                {
                    foreach (Match match in Regex.Matches(inputParameter.Value, @"\${(.*)}"))
                    {
                        string variableName = match.Groups[1].Value;
                        inputParameter.Value = inputParameter.Value.Replace(match.Value, serviceTaskContext.GetVariable<string>(variableName));
                    }

                    serviceTaskContext.SetVariable(inputParameter.Name, inputParameter.Value);
                }

                serviceTask.Execute(serviceTaskContext);

                foreach(var outputParameter in serviceTaskBpmnElement.OutputParameters)
                {
                    var outputValue = serviceTaskContext.GetVariable<string>(outputParameter.Name);
                    var outputVariableName = Regex.Match(outputParameter.Value, @"\${(.*)}").Groups[1].Value;

                    serviceTaskContext.ExecutionScope.ParentScope.SetVariable(outputVariableName, outputValue);
                }
            }

            if(currentBpmnElement is BpmnSubProcess)
            {
                StartProcess(((BpmnSubProcess)currentBpmnElement).Artifacts.OfType<BpmnStartEvent>());
                return;
            }

            if(currentBpmnElement is BpmnEndEvent)
            {
                processInstance.Tokens[command.Token.Id].Status = TokenStatus.Inactive;
                var bpmnEndEvent = (BpmnEndEvent)currentBpmnElement;
                if(bpmnEndEvent.ParentBpmnElement is BpmnSubProcess)
                {
                    var subProcess = (BpmnSubProcess)bpmnEndEvent.ParentBpmnElement;

                    lock (processInstance.Tokens)
                    {
                        if(bpmnEndEvent.Id == "sub_end_event")
                        {
                            var subsubProcessToken = processInstance.Tokens.FirstOrDefault(t => t.Value.CurrentElement.Id == "sub_sub_process_1");
                        }

                        var activeTokensCount = processInstance.Tokens.Count(t => t.Value.CurrentElement.ParentBpmnElement.Id == subProcess.Id && t.Value.Status != TokenStatus.Inactive);
                        if (activeTokensCount == 0)
                        {
                            var subProcessToken = processInstance.Tokens.First(t => t.Value.CurrentElement.Id == subProcess.Id);
                            commands.Add(new ProceedTokenCommand() { Token = subProcessToken.Value });
                        }
                    }
                }
            }

            if (currentBpmnElement is BpmnGateway)
            {
                var gateway = (BpmnGateway)currentBpmnElement;
                int numberOfTokensReceivedOnGateway = processInstance.Tokens.Count(t => t.Value.CurrentElement.Id == command.Token.CurrentElement.Id);
                if (gateway.Incoming.Count == numberOfTokensReceivedOnGateway)
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
