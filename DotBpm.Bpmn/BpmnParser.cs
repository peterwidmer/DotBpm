﻿using DotBpm.Bpmn.BpmnModel;
using DotBpm.Bpmn.CamundaExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace DotBpm.Bpmn
{
    public class BpmnParser
    {
        public const string NS_BPMNMODEL = "http://www.omg.org/spec/BPMN/20100524/MODEL";
        public const string NS_CAMUNDA = "http://camunda.org/schema/1.0/bpmn";

        private XDocument bpmnDocument;
        private List<BpmnProcess> Processes = new List<BpmnProcess>();

        public BpmnParser(XDocument bpmnDocument)
        {
            this.bpmnDocument = bpmnDocument;
        }

        public BpmnProcess Parse()
        {
            foreach(var processElement in bpmnDocument.Root.Elements("{" + NS_BPMNMODEL + "}" + "process"))
            {
                var process = ParseProcess(processElement);
                process.IndexArtifacts();
                Processes.Add(process);
            }

            return Processes.FirstOrDefault();
        }

        private BpmnProcess ParseProcess(XElement processElement)
        {
            BpmnProcess process = new BpmnProcess();
            process.Id = processElement.Attribute("id")?.Value;
            process.IsExecutable = BpmnXmlHelper.GetAttributeBoolean(processElement, "isExecutable");

            ParseArtifacts(process.Artifacts, processElement, process);

            return process;
        }

        private void ParseArtifacts(List<BpmnBaseElement> artifacts, XElement processElement, BpmnBaseElement parentBpmnElement)
        {
            ParseStartEvents(artifacts, processElement);
            ParseTasks(artifacts, processElement);
            ParseServiceTasks(artifacts, processElement);
            ParseSequenceFlows(artifacts, processElement);
            ParseEndEvents(artifacts, processElement);
            ParseParallelGateway(artifacts, processElement);
            ParseExclusiveGateway(artifacts, processElement);
            ParseSubProcess(artifacts, processElement);
            foreach(var artifact in artifacts)
            {
                artifact.ParentBpmnElement = parentBpmnElement;
            }
        }

        private void ParseSubProcess(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var subProcessElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "subProcess"))
            {
                var subProcess = new BpmnSubProcess();
                subProcess.Id = subProcessElement.Attribute("id")?.Value;
                ParseBpmnFlowNode(subProcess, subProcessElement);
                ParseArtifacts(subProcess.Artifacts, subProcessElement, subProcess);
                artifacts.Add(subProcess);
            }
        }

        private void ParseParallelGateway(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var parallelGatewayElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "parallelGateway"))
            {
                var parallelGateway = new BpmnParallelGateway();
                ParseBpmnFlowNode(parallelGateway, parallelGatewayElement);
                artifacts.Add(parallelGateway);
            }
        }

        private void ParseExclusiveGateway(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var exclusiveGatewayElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "exclusiveGateway"))
            {
                var exclusiveGateway = new BpmnExclusiveGateway();
                ParseBpmnFlowNode(exclusiveGateway, exclusiveGatewayElement);
                artifacts.Add(exclusiveGateway);
            }
        }

        private void ParseStartEvents(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var startEventElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "startEvent"))
            {
                var startEvent = new BpmnStartEvent();
                ParseBpmnFlowNode(startEvent, startEventElement);
                artifacts.Add(startEvent);
            }
        }

        private void ParseEndEvents(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var endEventElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "endEvent"))
            {
                var endEvent = new BpmnEndEvent();                
                ParseBpmnFlowNode(endEvent, endEventElement);
                artifacts.Add(endEvent);
            }
        }

        private void ParseTasks(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var taskElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "task"))
            {
                var task = new BpmnTask();
                ParseTask(taskElement, task);
                artifacts.Add(task);
            }
        }

        private void ParseServiceTasks(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var serviceTaskElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "serviceTask"))
            {
                var serviceTask = new BpmnServiceTask();
                serviceTask.Class = serviceTaskElement.Attribute("{" + NS_CAMUNDA + "}" + "class")?.Value;
                ParseTask(serviceTaskElement, serviceTask);
                artifacts.Add(serviceTask);
            }
        }

        private void ParseTask(XElement taskElement, BpmnTask task)
        {
            ParseBpmnFlowNode(task, taskElement);   
        }

        public void ParseSequenceFlows(List<BpmnBaseElement> artifacts, XElement processElement)
        {
            foreach (var sequenceFlowElement in processElement.Elements("{" + NS_BPMNMODEL + "}" + "sequenceFlow"))
            {
                var sequenceFlow = new BpmnSequenceFlow();
                sequenceFlow.SourceRef = sequenceFlowElement.Attribute("sourceRef")?.Value;
                sequenceFlow.TargetRef = sequenceFlowElement.Attribute("targetRef")?.Value;
                ParseBpmnConditionExpression(sequenceFlow, sequenceFlowElement);
                ParseBpmnFlowElement(sequenceFlow, sequenceFlowElement);
                artifacts.Add(sequenceFlow);
            }
        }

        private void ParseBpmnConditionExpression(BpmnSequenceFlow bpmnSequenceFlow, XElement sequenceFlowElement)
        {
            var conditionExpressionElement = sequenceFlowElement.Elements("{" + NS_BPMNMODEL + "}" + ("conditionExpression")).FirstOrDefault();
            if(conditionExpressionElement != null)
            {
                ParseBpmnBaseElement(bpmnSequenceFlow.ConditionExpression, conditionExpressionElement);
                bpmnSequenceFlow.ConditionExpression.Language = conditionExpressionElement.Attribute("language")?.Value;
                bpmnSequenceFlow.ConditionExpression.Body = conditionExpressionElement.Value;
            }
        }

        private void ParseBpmnFlowElement(BpmnFlowElement bpmnFlowElement, XElement bpmnElement)
        {
            ParseBpmnBaseElement(bpmnFlowElement, bpmnElement);
            bpmnFlowElement.Name = bpmnElement.Attribute("name")?.Value;
        }

        private void ParseBpmnBaseElement(BpmnBaseElement bpmnBase, XElement bpmnBaseElememt)
        {
            bpmnBase.Id = bpmnBaseElememt.Attribute("id")?.Value;
        }

        private void ParseBpmnFlowNode(BpmnFlowNode bpmnFlowNode, XElement bpmnElement)
        {
            foreach (var incoming in bpmnElement.Elements("{" + NS_BPMNMODEL + "}" + "incoming"))
            {
                bpmnFlowNode.Incoming.Add(incoming.Value);
            }

            foreach (var outgoing in bpmnElement.Elements("{" + NS_BPMNMODEL + "}" + "outgoing"))
            {
                bpmnFlowNode.Outgoing.Add(outgoing.Value);
            }

            ParseCamundaFlowNodeExtensionElements(bpmnFlowNode, bpmnElement);
            ParseBpmnFlowElement(bpmnFlowNode, bpmnElement);
        }

        private static void ParseCamundaFlowNodeExtensionElements(BpmnFlowNode bpmnFlowNode, XElement bpmnElement)
        {
            foreach (var extensionElements in bpmnElement.Elements("{" + NS_BPMNMODEL + "}" + "extensionElements"))
            {
                var inputOutputElement = extensionElements.Element("{" + NS_CAMUNDA + "}" + "inputOutput");
                if (inputOutputElement != null)
                {
                    foreach (var inputParameterElement in inputOutputElement.Elements("{" + NS_CAMUNDA + "}" + "inputParameter"))
                    {
                        bpmnFlowNode.InputParameters.Add(new ElementParameter(inputParameterElement.Attribute("name").Value, inputParameterElement.Value));
                    }

                    foreach (var outputParameterElement in inputOutputElement.Elements("{" + NS_CAMUNDA + "}" + "outputParameter"))
                    {
                        bpmnFlowNode.OutputParameters.Add(new ElementParameter(outputParameterElement.Attribute("name").Value, outputParameterElement.Value));
                    }
                }
            }
        }
    }
}
