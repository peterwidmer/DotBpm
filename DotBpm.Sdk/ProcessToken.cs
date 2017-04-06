using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Sdk
{
    public class ProcessToken
    {
        public string CurrentElementId { get; set; }
        public object ScopeData { get; set; } 

        public ProcessToken(string currentElementId)
        {
            CurrentElementId = currentElementId;
        }
    }
}
