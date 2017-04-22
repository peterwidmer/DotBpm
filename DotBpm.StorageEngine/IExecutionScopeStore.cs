using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.StorageEngine
{
    public interface IExecutionScopeStore
    {
        ExecutionScope Create(Guid processInstanceId, string bpmnElementId);
        ExecutionScope Create(Guid processInstanceId, string bpmnElementId, string parentBpmnElementId);
        void Save(Guid processInstanceId, ExecutionScope executionScope);
        ExecutionScope Load(Guid processInstanceId, string bpmnElementId);
    }
}
