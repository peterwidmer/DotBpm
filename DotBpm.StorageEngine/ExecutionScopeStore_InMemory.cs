using System;
using System.Collections.Generic;
using System.Text;
using DotBpm.Sdk;

namespace DotBpm.StorageEngine
{
    public class ExecutionScopeStore_InMemory : IExecutionScopeStore
    {
        private List<ExecutionScope> executionScopes;

        public ExecutionScopeStore_InMemory()
        {
            executionScopes = new List<ExecutionScope>();
        }

        public ExecutionScope Create()
        {
            var executionScope = new ExecutionScope();
            lock(executionScopes)
            {
                executionScopes.Add(executionScope);
                executionScope.Id = executionScopes.Count - 1;
            }

            return executionScope;
        }

        public ExecutionScope Create(int parentScopeId)
        {
            var executionScope = Create();
            executionScopes[executionScope.Id].ParentScope = executionScopes[parentScopeId];
            return executionScope;
        }

        public ExecutionScope Load(int Id)
        {
            return executionScopes[Id];
        }

        public void Save(ExecutionScope executionScope)
        {
            executionScopes[executionScope.Id] = executionScope;
        }
    }
}
