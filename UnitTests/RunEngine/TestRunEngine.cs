﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotBpm.Sdk;
using StorageEngine;
using System.Xml;
using DotBpm.StorageEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotBpm.Engines;
using UnitTests.RunEngine;

namespace UnitTests
{
    [TestClass]
    public class TestRunEngine
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
        public void TestRunEngine_SimpleRun()
        {
            var processInstance = RunEngine_TestHelper.CreateProcessInstance(processEngine, processDefinitionStore, "UnitTests.Bpmn.diagram.bpmn", "diagram_process");

            dotBpmEngine.ExecuteProcess(processInstance);
        }

        [TestMethod]
        public void TestRunEngine_ParallelGatewayRun()
        {
            var processInstance = RunEngine_TestHelper.CreateProcessInstance(processEngine, processDefinitionStore, "UnitTests.Bpmn.diagram_parallelgateway.bpmn", "diagram_parallelgateway_process");

            dotBpmEngine.ExecuteProcess(processInstance);
        }

        [TestMethod]
        public void TestRunEngine_ExclusiveGatewayRun()
        {
            var processInstance = RunEngine_TestHelper.CreateProcessInstance(processEngine, processDefinitionStore, "UnitTests.Bpmn.diagram_exclusivegateway.bpmn", "diagram_exclusivegateway");
            var variables = new Dictionary<string, object>() { { "decision", "test_task_2" } };
            dotBpmEngine.ExecuteProcess(processInstance, variables);
        }

        [TestMethod]
        public void TestRunEngine_SubProcessRun()
        {
            var processInstance = RunEngine_TestHelper.CreateProcessInstance(processEngine, processDefinitionStore, "UnitTests.Bpmn.diagram_subprocess.bpmn", "diagram_subprocess_process");

            dotBpmEngine.ExecuteProcess(processInstance);
        }
    }
}
