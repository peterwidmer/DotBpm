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

            foreach (var token in processInstance.Tokens)
            {
                commands.Add(new ProceedTokenCommand() { Token = token });
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
                var currentBpmnElement = processInstance.BpmnProcess.Elements.First(t => t.Id == ((ProceedTokenCommand)command).Token.CurrentElementId);
                if(currentBpmnElement is BpmnSequenceFlow)
                {
                    // Next Element
                }
                else
                {
                    // Must be a sequenceflow
                }
            }
        }
    }
}
