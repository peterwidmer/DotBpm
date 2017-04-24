using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.StorageEngine
{
    public interface IElementLogStore
    {
        ElementLog Create(Guid processInstanceId, string bpmnElementId);
        void Save(Guid processInstanceId, ElementLog elementLog);
        ElementLog Load(Guid processInstanceId, string bpmnElementId);
    }
}
