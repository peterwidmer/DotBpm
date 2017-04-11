using DotBpm.Bpmn.BpmnModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Sdk
{
    public class ProcessInstance
    {
        public Guid Id { get; set; }
        public BpmnProcess BpmnProcess { get; set; }
        public Dictionary<Guid, ProcessToken> Tokens { get; set; }

        public ProcessInstance()
        {
            Tokens = new Dictionary<Guid, ProcessToken>();
        }
    }
}
