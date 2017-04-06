using DotBpmSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageEngine
{
    public class ProcessDefinitionStore_InMemory : IProcessDefinitionStore
    {
        private Dictionary<string, List<ProcessDefinition>> processDefinitions = new Dictionary<string, List<ProcessDefinition>>();

        public void Save(string processName, string bpmnContent)
        {
            if(!processDefinitions.ContainsKey(processName))
            {
                processDefinitions.Add(processName, new List<ProcessDefinition>());
            }
            var existingProcessDefinitions = processDefinitions[processName];

            existingProcessDefinitions.Add(new ProcessDefinition()
            {
                BpmnContent = bpmnContent,
                ProcessName = processName,
                Version = existingProcessDefinitions.Count + 1
            });
        }

        public ProcessDefinition Load(string processName)
        {
            if(processDefinitions.ContainsKey(processName) && processDefinitions[processName].Count > 0)
            {
                return processDefinitions[processName][processDefinitions[processName].Count - 1];
            }
            else
            {
                throw new ArgumentOutOfRangeException("There's no processdefinition stored for process " + processName);
            }

        }

        public ProcessDefinition Load(string processName, int version)
        {
            if (processDefinitions.ContainsKey(processName) && processDefinitions[processName].Count > 0)
            {
                return processDefinitions[processName].FirstOrDefault(t=> t.Version == version);
            }
            else
            {
                throw new ArgumentOutOfRangeException("There's no processdefinition stored for process " + processName);
            }
        }
    }
}
