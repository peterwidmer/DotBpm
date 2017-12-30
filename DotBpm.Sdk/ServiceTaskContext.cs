using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public class ServiceTaskContext
    {
        public ProcessToken Token { get; }
        public ExecutionScope ExecutionScope { get; set; }

        public ServiceTaskContext(ProcessToken token, ExecutionScope executionScope)
        {
            Token = token;
            ExecutionScope = executionScope;
        }

        public T GetVariable<T>(string variable)
        {
            return ExecutionScope.GetVariable<T>(variable);
        }

        public void SetVariable(string variableName, object variableValue)
        {
            ExecutionScope.SetVariable(variableName, variableValue);
        }
    }
}
