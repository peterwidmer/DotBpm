using DotBpm.Bpmn.CamundaExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnFlowNode : BpmnFlowElement
    {
        public List<string> Incoming { get; set; } = new List<string>();
        public List<string> Outgoing { get; set; } = new List<string>();

        public List<ElementParameter> InputParameters { get; set; } = new List<ElementParameter>();
        public List<ElementParameter> OutputParameters { get; set; } = new List<ElementParameter>();
    }
}
