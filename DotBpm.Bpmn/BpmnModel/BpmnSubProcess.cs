using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnSubProcess : BpmnActivity
    {
        public List<BpmnBaseElement> Artifacts { get; set; }

        public BpmnSubProcess()
        {
            Artifacts = new List<BpmnBaseElement>();
        }
    }
}
