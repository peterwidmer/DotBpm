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

namespace Engines
{
    public class RunEngine
    {
        private ProcessInstance processInstance;
        private BlockingCollection<Command> commands = new BlockingCollection<Command>();

        public RunEngine(ProcessInstance processInstance)
        {
            this.processInstance = processInstance;
        }

        public async Task ExecuteProcess()
        {

            foreach (var startEvent in processInstance.BpmnProcess.Elements.OfType<BpmnStartEvent>())
            {
                var token = new ProcessToken(startEvent.Id);
                processInstance.Tokens.Add(token.Id, token);
                commands.Add(new ExecuteTokenCommand() { Token =  token});                
            }

            await Task.Run(() =>
            {
                foreach (var command in commands.GetConsumingEnumerable())
                {
                    BackgroundWorker bgw = new BackgroundWorker();
                    bgw.DoWork += Bgw_DoWork;
                    bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
                    bgw.RunWorkerAsync(command);                    
                }
            });
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
                var currentBpmnElement = processInstance.BpmnProcess.Elements.First(t => t.Id == command.Token.CurrentElementId);
                if (currentBpmnElement is BpmnSequenceFlow)
                {
                    // Next must be an Element
                    Trace.WriteLine("Token on : " + command.Token.CurrentElementId);
                    
                    var sequenceFlow = (BpmnSequenceFlow)currentBpmnElement;
                    
                    var token = new ProcessToken(sequenceFlow.TargetRef);
                    processInstance.Tokens.Add(token.Id, token);
                    commands.Add(new ExecuteTokenCommand() { Token = token });
                }
                else if (currentBpmnElement is BpmnFlowNode)
                {
                    // Create token for each outgoing and deactivate current token
                    var flowNode = (BpmnFlowNode)currentBpmnElement;

                    foreach (var outgoing in flowNode.Outgoing)
                    {
                        var token = new ProcessToken(outgoing);
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

        private void HandleExecuteTokenCommand(ExecuteTokenCommand command)
        {
            Trace.WriteLine("Token on : " + command.Token.CurrentElementId);
            processInstance.Tokens[command.Token.Id].Status = TokenStatus.InExecution;
            var currentBpmnElement = processInstance.BpmnProcess.Elements.First(t => t.Id == command.Token.CurrentElementId);
            if(currentBpmnElement is BpmnServiceTask)
            {
                var sleepTask = new SleepTask();
                sleepTask.Execute(new ServiceTaskContext(command.Token));
            }

            if (currentBpmnElement is BpmnParallelGateway)
            {
                var parallelGateway = (BpmnParallelGateway)currentBpmnElement;
                int numberOfTokensReceivedOnGateway = processInstance.Tokens.Count(t => t.Value.CurrentElementId == command.Token.CurrentElementId);
                if(parallelGateway.Incoming.Count == numberOfTokensReceivedOnGateway)
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
