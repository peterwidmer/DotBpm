using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnBaseElement
    {
        public BpmnBaseElement ParentBpmnElement { get; set; }
        public string Id { get; set; }
    }
}
