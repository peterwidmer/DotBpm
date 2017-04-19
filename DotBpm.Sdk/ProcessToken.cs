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
        public string CurrentElementId { get; }
        public TokenStatus Status { get; set; }

        public ProcessToken(string currentElementId)
        {
            Id = Guid.NewGuid();
            CurrentElementId = currentElementId;
        }

        public ProcessToken(string currentElementId, TokenStatus status)
        {
            Id = Guid.NewGuid();
            CurrentElementId = currentElementId;
            Status = status;
        }
    }
}
