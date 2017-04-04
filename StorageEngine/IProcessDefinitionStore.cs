using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageEngine
{
    public interface IProcessDefinitionStore
    {
        void Save(string processName, string bpmnContent);
        ProcessDefinition Load(string processName);
        ProcessDefinition Load(string processName, int version);
    }
}
