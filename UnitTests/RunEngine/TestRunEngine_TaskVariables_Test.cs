using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotBpm.Sdk;
using StorageEngine;
using System.Xml;
using DotBpm.StorageEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotBpm.Engines;

namespace UnitTests.RunEngine
{
    [TestClass]
    public class TestRunEngine_TaskVariables_Test
    {
        private static IExecutionScopeStore executionScopeStore;
        private static IServiceTaskEngine serviceTaskEngine;
        private static IProcessEngine processEngine;
        private static IProcessDefinitionStore processDefinitionStore;

        private static DotBpmEngine dotBpmEngine;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            executionScopeStore = new ExecutionScopeStore_InMemory();
            serviceTaskEngine = new ServiceTaskEngine();
            processDefinitionStore = new ProcessDefinitionStore_InMemory();
            processEngine = new ProcessEngine(processDefinitionStore);

            dotBpmEngine = new DotBpmEngine(executionScopeStore, serviceTaskEngine);
        }

        [TestMethod]
        public void TestRunEngine_ExclusiveGatewayRun()
        {
            // I'm working on it, so its ok to fail
            var processInstance = RunEngine_TestHelper.CreateProcessInstance(processEngine, processDefinitionStore, "UnitTests.Bpmn.diagram_task_variables_test.bpmn", "diagram_task_variables_test");
            var variables = new Dictionary<string, object>()
            {
                { "value1", 3 },
                { "value2", 8 },
            };
            dotBpmEngine.ExecuteProcess(processInstance, variables);
        }

    }
}
