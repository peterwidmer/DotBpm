using System;
using System.Collections.Generic;
using System.Text;
using DotBpm.Sdk;

namespace DotBpm.StorageEngine
{
    public class ExecutionScopeStore_InMemory : IExecutionScopeStore
    {
        private Dictionary<string, ExecutionScope> executionScopes;

        public ExecutionScopeStore_InMemory()
        {
            executionScopes = new Dictionary<string, ExecutionScope>();
        }

        public ExecutionScope Create(Guid processInstanceId, string bpmnElementId)
        {
            var executionScope = new ExecutionScope(bpmnElementId);
            lock(executionScopes)
            {
                executionScopes.Add(StoreHelper.Key(processInstanceId, bpmnElementId), executionScope);
            }

            return executionScope;
        }

        public ExecutionScope Create(Guid processInstanceId, string bpmnElementId, string parentBpmnElementId)
        {
            var executionScope = Create(processInstanceId, bpmnElementId);
            executionScope.ParentScope = executionScopes[StoreHelper.Key(processInstanceId, parentBpmnElementId)];

            return executionScope;
        }

        public ExecutionScope Load(Guid processInstanceId, string bpmnElementId)
        {
            return executionScopes[StoreHelper.Key(processInstanceId, bpmnElementId)];
        }

        public void Save(Guid processInstanceId, ExecutionScope executionScope)
        {
            executionScopes[StoreHelper.Key(processInstanceId, executionScope.ScopeOfBpmnElementId)] = executionScope;
        }

    }
}
