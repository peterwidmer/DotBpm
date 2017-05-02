using DotBpm.Bpmn.CamundaExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnServiceTask : BpmnTask
    {
        public string Class { get; set; }
        public List<ElementParameter> InputParameters { get; set; }
        public List<ElementParameter> OutputParameters { get; set; }
    }
}
