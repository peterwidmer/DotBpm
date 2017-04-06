using DotBpm.Bpmn;
using DotBpm.Bpmn.BpmnModel;
using DotBpm.Sdk;
using StorageEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProcessEngine
{
    public class ProcessEngine
    {
        private IProcessDefinitionStore processDefinitionStore;

        public ProcessEngine(IProcessDefinitionStore processDefinitionStore)
        {
            this.processDefinitionStore = processDefinitionStore;
        }

        public void StartProcess(string processName)
        {
            var bpmnProcess = LoadBpmnProcess(processName);
            var processInstance = new ProcessInstance()
            {
                Id = Guid.NewGuid(),
                BpmnProcess = bpmnProcess,
                Tokens = bpmnProcess.Elements.OfType<BpmnStartEvent>().Select(t => new ProcessToken(t.Id)).ToList()
            };
        }

        private BpmnProcess LoadBpmnProcess(string processName)
        {
            var processDefinition = processDefinitionStore.Load(processName);

            var bpmnDocument = new XmlDocument();
            bpmnDocument.LoadXml(processDefinition.BpmnContent);

            var bpmnParser = new BpmnParser(bpmnDocument);
            return bpmnParser.Parse();
        }
    }
}
