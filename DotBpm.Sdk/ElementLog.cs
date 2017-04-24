using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public class ElementLog
    {
        public string BpmnElementId { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }

        public ElementLog(string bpmnElementId)
        {
            BpmnElementId = bpmnElementId;
            Started = DateTime.Now;
        }
    }
}
