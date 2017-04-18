using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.StorageEngine
{
    public interface IExecutionScopeStore
    {
        ExecutionScope Create();
        ExecutionScope Create(int parentScopeId);
        void Save(ExecutionScope executionScope);
        ExecutionScope Load(int Id);
    }
}
