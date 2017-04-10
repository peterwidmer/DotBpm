using DotBpm.Sdk.Commands;
using DotBpm.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotBpm.Bpmn.BpmnModel;

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
                processInstance.Tokens.Add(startEvent.Id, token);
                commands.Add(new ExecuteTokenCommand() { Token =  token});                
            }

            await Task.Run(() =>
            {
                foreach (var command in commands.GetConsumingEnumerable())
                {
                    ExecuteCommand(command);
                    lock(processInstance.Tokens)
                    {
                        if(processInstance.Tokens.Count(t=> t.Value.Status == TokenStatus.Active) == 0)
                        {
                            break;
                        }
                    }
                }
            });
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
                    var sequenceFlow = (BpmnSequenceFlow)currentBpmnElement;
                    
                    var token = new ProcessToken(sequenceFlow.TargetRef);
                    processInstance.Tokens.Add(sequenceFlow.TargetRef, token);
                    commands.Add(new ExecuteTokenCommand() { Token = token });
                }
                else if (currentBpmnElement is BpmnFlowNode)
                {
                    // Create token for each outgoing and deactivate current token
                    var flowNode = (BpmnFlowNode)currentBpmnElement;

                    foreach (var outgoing in flowNode.Outgoing)
                    {
                        var token = new ProcessToken(outgoing);
                        processInstance.Tokens.Add(outgoing, token);
                        commands.Add(new ProceedTokenCommand() { Token = token });
                    }
                }
                else
                {
                    throw new Exception("Unexpected BpmnElement on ExecuteCommand " + currentBpmnElement.GetType().Name);
                }

                processInstance.Tokens[currentBpmnElement.Id].Status = TokenStatus.Inactive;
            }
        }

        private void HandleExecuteTokenCommand(ExecuteTokenCommand command)
        {
            processInstance.Tokens[command.Token.CurrentElementId].Status = TokenStatus.InExecution;
        }
    }
}
