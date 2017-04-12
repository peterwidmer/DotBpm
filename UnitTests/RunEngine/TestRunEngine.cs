using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotBpm.Sdk;
using Engines;
using StorageEngine;
using DotBpm.Bpmn;
using System.Xml;

namespace UnitTests
{
    [TestClass]
    public class TestRunEngine
    {
        [TestMethod]
        public void TestRunEngine_SimpleRun()
        {
            var processInstance = CreateProcessInstance("UnitTests.Bpmn.diagram.bpmn", "diagram_process");

            RunEngine runEngine = new RunEngine(processInstance);
            var task = runEngine.ExecuteProcess();
            task.Wait();
        }

        [TestMethod]
        public void TestRunEngine_ParallelGatewayRun()
        {
            var processInstance = CreateProcessInstance("UnitTests.Bpmn.diagram_parallelgateway.bpmn", "diagram_parallelgateway_process");

            RunEngine runEngine = new RunEngine(processInstance);
            var task = runEngine.ExecuteProcess();
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
