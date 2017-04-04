using Bpmn;
using DotBpmSdk;
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
            var processDefinition = processDefinitionStore.Load(processName);

            var bpmnDocument = new XmlDocument();
            bpmnDocument.LoadXml(processDefinition.BpmnContent);

            var bpmnParser = new BpmnParser(bpmnDocument);
            var process = bpmnParser.Parse();
        }
    }
}
