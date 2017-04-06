using Bpmn.BpmnModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bpmn
{
    public class BpmnParser
    {
        public const string NS_BPMNMODEL = "http://www.omg.org/spec/BPMN/20100524/MODEL";

        private XmlDocument bpmnDocument;
        private List<BpmnProcess> Processes = new List<BpmnProcess>();

        public BpmnParser(XmlDocument bpmnDocument)
        {
            this.bpmnDocument = bpmnDocument;
        }

        public BpmnProcess Parse()
        {
            foreach(var processElement in bpmnDocument.GetElementsByTagName("process", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var process = ParseProcess(processElement);
                Processes.Add(process);
            }

            return Processes.FirstOrDefault();
        }

        private BpmnProcess ParseProcess(XmlElement processElement)
        {
            BpmnProcess process = new BpmnProcess();
            process.Id = processElement.GetAttribute("id");
            process.IsExecutable = BpmnXmlHelper.GetAttributeBoolean(processElement, "isExecutable");

            ParseStartEvents(process, processElement);
            ParseTasks(process, processElement);
            ParseSequenceFlows(process, processElement);

            return process;
        }

        private void ParseStartEvents(BpmnProcess process, XmlElement processElement)
        {
            foreach(var startEventElement in processElement.GetElementsByTagName("startEvent", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var startEvent = new BpmnStartEvent();
                BaseParser(startEvent, startEventElement);
                process.Elements.Add(startEvent);
            }
        }

        private void ParseTasks(BpmnProcess process, XmlElement processElement)
        {
            foreach (var taskElement in processElement.GetElementsByTagName("task", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var task = new BpmnTask();
                BaseParser(task, taskElement);
                process.Elements.Add(task);
            }
        }

        public void ParseSequenceFlows(BpmnProcess process, XmlElement processElement)
        {
            foreach (var sequenceFlowElement in processElement.GetElementsByTagName("sequenceFlow", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var sequenceFlow = new BpmnSequenceFlow();
                sequenceFlow.SourceRef = sequenceFlowElement.GetAttribute("sourceRef");
                sequenceFlow.TargetRef = sequenceFlowElement.GetAttribute("targetRef");
                BaseParser(sequenceFlow, sequenceFlowElement);
                process.Elements.Add(sequenceFlow);
            }
        }

        private void BaseParser(BpmnFlowElement bpmnBase, XmlElement bpmnElement)
        {
            bpmnBase.Id = bpmnElement.GetAttribute("id");
            bpmnBase.Name = bpmnElement.GetAttribute("name");
        }
    }
}
