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
            string bpmnProcessContent = TestHelper.GetEmbeddedResourceAsString("UnitTests.Bpmn.diagram.bpmn");

            var processDefinitionStore = new ProcessDefinitionStore_InMemory();
            processDefinitionStore.Save("diagram_process", bpmnProcessContent);

            var processEngine = new ProcessEngine(processDefinitionStore);
            var processInstance = processEngine.GetProcessInstance("diagram_process");

            RunEngine runEngine = new RunEngine(processInstance);
            var task = runEngine.ExecuteProcess();
            task.Wait();
        }
    }
}
