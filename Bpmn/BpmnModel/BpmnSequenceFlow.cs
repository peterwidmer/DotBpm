using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpmn.BpmnModel
{
    public class BpmnSequenceFlow : BpmnFlowElement
    {
        public string SourceRef { get; set; }
        public string TargetRef { get; set; }
    }
}
