using DotBpm.Bpmn;
using DotBpm.Bpmn.BpmnModel;
using DotBpm.Sdk;
using StorageEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engines
{
    public class ProcessEngine
    {
        private IProcessDefinitionStore processDefinitionStore;

        public ProcessEngine(IProcessDefinitionStore processDefinitionStore)
        {
            this.processDefinitionStore = processDefinitionStore;
        }

        public ProcessInstance GetProcessInstance(string processName)
        {
            var bpmnProcess = LoadBpmnProcess(processName);
            return new ProcessInstance()
            {
                Id = Guid.NewGuid(),
                BpmnProcess = bpmnProcess
            };
        }

        private BpmnProcess LoadBpmnProcess(string processName)
        {
            var processDefinition = processDefinitionStore.Load(processName);

            var bpmnDocument = XDocument.Parse(processDefinition.BpmnContent);

            var bpmnParser = new BpmnParser(bpmnDocument);
            return bpmnParser.Parse();
        }
    }
}
