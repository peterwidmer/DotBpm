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
            return (T)FindVariableInScopeHierarchy(ExecutionScope, variable);
        }

        private object FindVariableInScopeHierarchy(ExecutionScope currentScope, string variable)
        {
            if(currentScope.Variables.ContainsKey(variable))
            {
                return currentScope.Variables[variable];
            }
            else if(currentScope.ParentScope != null)
            {
                return FindVariableInScopeHierarchy(currentScope.ParentScope, variable);
            }
            else
            {
                throw new ArgumentOutOfRangeException("A variable with the name " + variable + " does not exist");
            }
        }
        
    }
}
