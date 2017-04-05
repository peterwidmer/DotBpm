using DotBpmSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessEngine
{
    public class RunEngine
    {
        private ProcessInstance processInstance;

        public RunEngine(ProcessInstance processInstance)
        {
            this.processInstance = processInstance;
        }

        public async Task ExecuteProcess()
        {
            while(processInstance.Tokens.Count != 0)
            {
                List<Task> taskList = new List<Task>();
                foreach (var token in processInstance.Tokens)
                {
                    var bpmnElement = processInstance.BpmnProcess.Elements.First(t => t.Id == token.CurrentElementId);
                    var task = new Task(() => { Console.Write("Execute " + token.CurrentElementId); });
                }

                await Task.WhenAll(taskList);
            }
        }
    }
}
