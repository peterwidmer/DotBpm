using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpmn.BpmnModel
{
    public class BpmnSequenceFlow : BpmnBase
    {
        public string SourceRef { get; set; }
        public string TargetRef { get; set; }
    }
}
