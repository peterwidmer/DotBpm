using System;
using System.Collections.Generic;
using System.Text;
using DotBpm.Sdk;

namespace DotBpm.StorageEngine
{
    public class ElementLogStore : IElementLogStore
    {
        private Dictionary<string, ElementLog> elementLogs;

        public ElementLogStore()
        {
            elementLogs = new Dictionary<string, ElementLog>();
        }

        public ElementLog Create(Guid processInstanceId, string bpmnElementId)
        {
            var elementLog = new ElementLog(bpmnElementId);
            lock (elementLogs)
            {
                elementLogs.Add(StoreHelper.Key(processInstanceId, bpmnElementId), elementLog);
            }

            return elementLog;
        }

        public ElementLog Load(Guid processInstanceId, string bpmnElementId)
        {
            return elementLogs[StoreHelper.Key(processInstanceId, bpmnElementId)];
        }

        public void Save(Guid processInstanceId, ElementLog elementLog)
        {
            elementLogs[StoreHelper.Key(processInstanceId, elementLog.BpmnElementId)] = elementLog;
        }
    }
}
