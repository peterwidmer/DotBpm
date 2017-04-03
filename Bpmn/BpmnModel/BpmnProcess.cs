using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpmn.BpmnModel
{
    public class BpmnProcess : BpmnBaseElement
    {
        public bool IsExecutable { get; set; }

        public List<BpmnStartEvent> StartEvents { get; set; }
        public List<BpmnTask> Tasks { get; set; }
        public List<BpmnSequenceFlow> SequenceFlows { get; set; }

        public BpmnProcess()
        {
            StartEvents = new List<BpmnStartEvent>();
            Tasks = new List<BpmnTask>();
            SequenceFlows = new List<BpmnSequenceFlow>();
        }
    }
}
