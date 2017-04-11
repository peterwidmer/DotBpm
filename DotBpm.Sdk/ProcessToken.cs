using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Sdk
{
    public class ProcessToken
    {
        public Guid Id { get; set; }
        public string CurrentElementId { get; }
        public object ScopeData { get; set; }
        public TokenStatus Status { get; set; }

        public ProcessToken(string currentElementId)
        {
            Id = Guid.NewGuid();
            CurrentElementId = currentElementId;
        }
    }
}
