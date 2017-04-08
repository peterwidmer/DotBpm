using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnFlowNode : BpmnFlowElement
    {
        public List<string> Incoming { get; set; }
        public List<string> Outgoing { get; set; }

        public BpmnFlowNode()
        {
            Incoming = new List<string>();
            Outgoing = new List<string>();
        }
    }
}
