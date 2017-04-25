using DotBpm.Bpmn.BpmnModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Sdk
{
    public class ProcessToken
    {
        public Guid Id { get; }
        public BpmnBaseElement CurrentElement { get; }
        public TokenStatus Status { get; set; }

        public ProcessToken(BpmnBaseElement currentElement)
        {
            Id = Guid.NewGuid();
            CurrentElement = currentElement;
        }

        public ProcessToken(BpmnBaseElement currentElement, TokenStatus status)
        {
            Id = Guid.NewGuid();
            CurrentElement = currentElement;
            Status = status;
        }
    }
}
