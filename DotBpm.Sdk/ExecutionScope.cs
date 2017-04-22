using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public class ExecutionScope
    {
        public string ScopeOfBpmnElementId { get; set; }
        public ExecutionScope ParentScope { get; set; }
        public Dictionary<string, object> Variables { get; set; }

        public ExecutionScope(string scopeOfBpmnElementId)
        {
            ScopeOfBpmnElementId = scopeOfBpmnElementId;
            Variables = new Dictionary<string, object>();
        }
    }
}
