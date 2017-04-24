using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.StorageEngine
{
    public class StoreHelper
    {
        public static string Key(Guid processInstanceId, string bpmnElementId)
        {
            return processInstanceId + bpmnElementId;
        }
    }
}
