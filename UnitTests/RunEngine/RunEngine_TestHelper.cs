using DotBpm.Engines;
using DotBpm.Sdk;
using StorageEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.RunEngine
{
    public class RunEngine_TestHelper
    {
        public static ProcessInstance CreateProcessInstance(IProcessEngine processEngine, IProcessDefinitionStore processDefinitionStore, string diagramFile, string processName)
        {
            string bpmnProcessContent = TestHelper.GetEmbeddedResourceAsString(diagramFile);
            processDefinitionStore.Save(processName, bpmnProcessContent);

            return processEngine.GetProcessInstance(processName);
        }
    }
}
