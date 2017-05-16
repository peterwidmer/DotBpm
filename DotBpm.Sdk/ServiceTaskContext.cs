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
            object result = FindVariableInScopeHierarchy(ExecutionScope, variable);
            if (typeof(T) == typeof(string))
            {
                result = result.ToString();
            }
            else if(typeof(T) == typeof(int))
            {
                result = Convert.ToInt32(result);
            }

            return (T)result;
        }

        public void SetVariable(string variableName, object variableValue)
        {
            bool variableHasBeenSet = SetVariableInScopeHierarchy(ExecutionScope, variableName, variableValue);
            if(!variableHasBeenSet)
            {
                ExecutionScope.Variables.Add(variableName, variableValue);
            }
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

        private bool SetVariableInScopeHierarchy(ExecutionScope currentScope, string variableName, object variableValue)
        {
            if (currentScope.Variables.ContainsKey(variableName))
            {
                currentScope.Variables[variableName] = variableValue;
                return true;
            }
            else if (currentScope.ParentScope != null)
            {
                return SetVariableInScopeHierarchy(currentScope.ParentScope, variableName, variableValue);
            }
            else
            {
                return false;
            }
        }

    }
}
