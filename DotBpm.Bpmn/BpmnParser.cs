using DotBpm.Bpmn.BpmnModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DotBpm.Bpmn
{
    public class BpmnParser
    {
        public const string NS_BPMNMODEL = "http://www.omg.org/spec/BPMN/20100524/MODEL";
        public const string NS_CAMUNDA = "http://camunda.org/schema/1.0/bpmn";

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
            ParseServiceTasks(process, processElement);
            ParseSequenceFlows(process, processElement);
            ParseEndEvents(process, processElement);
            ParseParallelGateway(process, processElement);
            ParseExclusiveGateway(process, processElement);

            return process;
        }

        private void ParseParallelGateway(BpmnProcess process, XmlElement processElement)
        {
            foreach (var parallelGatewayElement in processElement.GetElementsByTagName("parallelGateway", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var parallelGateway = new BpmnParallelGateway();
                ParseBpmnFlowNode(parallelGateway, parallelGatewayElement);
                process.Elements.Add(parallelGateway);
            }
        }

        private void ParseExclusiveGateway(BpmnProcess process, XmlElement processElement)
        {
            foreach (var exclusiveGatewayElement in processElement.GetElementsByTagName("exclusiveGateway", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var exclusiveGateway = new BpmnExclusiveGateway();
                ParseBpmnFlowNode(exclusiveGateway, exclusiveGatewayElement);
                process.Elements.Add(exclusiveGateway);
            }
        }

        private void ParseStartEvents(BpmnProcess process, XmlElement processElement)
        {
            foreach(var startEventElement in processElement.GetElementsByTagName("startEvent", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var startEvent = new BpmnStartEvent();
                ParseBpmnFlowNode(startEvent, startEventElement);
                process.Elements.Add(startEvent);
            }
        }

        private void ParseEndEvents(BpmnProcess process, XmlElement processElement)
        {
            foreach (var endEventElement in processElement.GetElementsByTagName("endEvent", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var endEvent = new BpmnEndEvent();                
                ParseBpmnFlowNode(endEvent, endEventElement);
                process.Elements.Add(endEvent);
            }
        }

        private void ParseTasks(BpmnProcess process, XmlElement processElement)
        {
            foreach (var taskElement in processElement.GetElementsByTagName("task", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var task = new BpmnTask();
                ParseTask(taskElement, task);
                process.Elements.Add(task);
            }
        }

        private void ParseServiceTasks(BpmnProcess process, XmlElement processElement)
        {
            foreach (var taskElement in processElement.GetElementsByTagName("serviceTask", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var serviceTask = new BpmnServiceTask();
                serviceTask.Class = taskElement.GetAttribute("class", NS_CAMUNDA);
                ParseTask(taskElement, serviceTask);
                process.Elements.Add(serviceTask);
            }
        }

        private void ParseTask(XmlElement taskElement, BpmnTask task)
        {
            ParseBpmnFlowNode(task, taskElement);   
        }

        public void ParseSequenceFlows(BpmnProcess process, XmlElement processElement)
        {
            foreach (var sequenceFlowElement in processElement.GetElementsByTagName("sequenceFlow", NS_BPMNMODEL).OfType<XmlElement>())
            {
                var sequenceFlow = new BpmnSequenceFlow();
                sequenceFlow.SourceRef = sequenceFlowElement.GetAttribute("sourceRef");
                sequenceFlow.TargetRef = sequenceFlowElement.GetAttribute("targetRef");
                ParseBpmnFlowElement(sequenceFlow, sequenceFlowElement);
                process.Elements.Add(sequenceFlow);
            }
        }

        private void ParseBpmnFlowElement(BpmnFlowElement bpmnFlowElement, XmlElement bpmnElement)
        {
            bpmnFlowElement.Id = bpmnElement.GetAttribute("id");
            bpmnFlowElement.Name = bpmnElement.GetAttribute("name");
        }

        private void ParseBpmnFlowNode(BpmnFlowNode bpmnFlowNode, XmlElement bpmnElement)
        {
            foreach(var incoming in bpmnElement.GetElementsByTagName("incoming", NS_BPMNMODEL).OfType<XmlElement>())
            {
                bpmnFlowNode.Incoming.Add(incoming.InnerText);
            }

            foreach (var outgoing in bpmnElement.GetElementsByTagName("outgoing", NS_BPMNMODEL).OfType<XmlElement>())
            {
                bpmnFlowNode.Outgoing.Add(outgoing.InnerText);
            }

            ParseBpmnFlowElement(bpmnFlowNode, bpmnElement);
        }
    }
}
