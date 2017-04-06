using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnProcess : BpmnBaseElement
    {
        public bool IsExecutable { get; set; }

        public List<BpmnBaseElement> Elements { get; set; }

        public BpmnProcess()
        {
            Elements = new List<BpmnBaseElement>();
        }
    }
}
