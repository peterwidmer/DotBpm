using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotBpm.Sdk;
using Engines;
using StorageEngine;
using DotBpm.Bpmn;
using System.Xml;
using DotBpm.StorageEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TestRunEngine
    {
        private static IExecutionScopeStore executionScopeStore;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            executionScopeStore = new ExecutionScopeStore_InMemory();
        }

        [TestMethod]
        public void TestRunEngine_SimpleRun()
        {
            var processInstance = CreateProcessInstance("UnitTests.Bpmn.diagram.bpmn", "diagram_process");

            ExecuteProcess(processInstance);
        }

        [TestMethod]
        public void TestRunEngine_ParallelGatewayRun()
        {
            var processInstance = CreateProcessInstance("UnitTests.Bpmn.diagram_parallelgateway.bpmn", "diagram_parallelgateway_process");

            ExecuteProcess(processInstance);
        }

        [TestMethod]
        public void TestRunEngine_ExclusiveGatewayRun()
        {
            var processInstance = CreateProcessInstance("UnitTests.Bpmn.diagram_exclusivegateway.bpmn", "diagram_exclusivegateway");
            var variables = new Dictionary<string, object>() { { "decision", "test_task_2" } };
            ExecuteProcess(processInstance, variables);
        }

        private static void ExecuteProcess(ProcessInstance processInstance)
        {
            ExecuteProcess(processInstance, new Dictionary<string, object>());
        }

        private static void ExecuteProcess(ProcessInstance processInstance, Dictionary<string, object> variables)
        {
            var runEngine = new RunEngine(processInstance, executionScopeStore);
            var task = runEngine.ExecuteProcess(variables);
            task.Wait();
        }

        public ProcessInstance CreateProcessInstance(string diagramFile, string processName)
        {
            string bpmnProcessContent = TestHelper.GetEmbeddedResourceAsString(diagramFile);

            var processDefinitionStore = new ProcessDefinitionStore_InMemory();
            processDefinitionStore.Save(processName, bpmnProcessContent);

            var processEngine = new ProcessEngine(processDefinitionStore);
            return processEngine.GetProcessInstance(processName);
        }
    }
}
