using DotBpm.Sdk.Commands;
using DotBpm.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotBpm.Bpmn.BpmnModel;

namespace ProcessEngine
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
            var currentBpmnElement = processInstance.BpmnProcess.Elements.First(t => t.Id == command.Token.CurrentElementId);
            if (currentBpmnElement is BpmnSequenceFlow)
            {
                // Next must be an Element
            }
            else if (currentBpmnElement is BpmnFlowNode)
            {
                // Next must be a sequenceflow
                var flowNode = (BpmnFlowNode)currentBpmnElement;
                foreach (var outgoing in flowNode.Outgoing)
                {
                    processInstance.Tokens.Add(outgoing, new ProcessToken(outgoing));
                }
            }
            else
            {
                throw new Exception("Unexpected BpmnElement on ExecuteCommand " + currentBpmnElement.GetType().Name);
            }
        }

        private void HandleExecuteTokenCommand(ExecuteTokenCommand command)
        {

        }
    }
}
