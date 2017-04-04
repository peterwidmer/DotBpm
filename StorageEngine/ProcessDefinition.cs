using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageEngine
{
    public class ProcessDefinition
    {
        public int Version { get; set; }
        public string ProcessName { get; set; }
        public string BpmnContent { get; set; }
    }
}
