using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public class ExecutionScope
    {
        public int Id { get; set; }
        public ExecutionScope ParentScope { get; set; }
        public Dictionary<string, object> Variables { get; set; }

        public ExecutionScope()
        {
            Variables = new Dictionary<string, object>();
        }
    }
}
