using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using DotBpm.Bpmn;
using System.Linq;
using DotBpm.Bpmn.BpmnModel;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoadDiagram_Test()
        {
            var bpmnDocument = new XmlDocument(); 
            bpmnDocument.LoadXml(TestHelper.GetEmbeddedResourceAsString("UnitTests.Bpmn.diagram.bpmn"));

            var bpmnParser = new BpmnParser(bpmnDocument);
            var process = bpmnParser.Parse();

            Assert.AreEqual("Process_1", process.Id);

            var startEvent1 = (BpmnStartEvent)process.Elements.FirstOrDefault(t => t.Id == "startevent_1");
            Assert.IsNotNull(startEvent1);
            Assert.AreEqual("Start Event 1", startEvent1.Name);
            Assert.IsTrue(startEvent1.Outgoing.Contains("start_task_1"));

            var task1 = (BpmnTask)process.Elements.FirstOrDefault(t => t.Id == "test_task_1");
            Assert.IsNotNull(task1);
            Assert.AreEqual("Test Task 1", task1.Name);
            Assert.IsTrue(task1.Incoming.Contains("start_task_1"));
            Assert.IsTrue(task1.Outgoing.Contains("task_1_end"));

            var sequenceFlow_StartEvent_Task1 = (BpmnSequenceFlow)process.Elements.FirstOrDefault(t => t.Id == "start_task_1");
            Assert.IsNotNull(sequenceFlow_StartEvent_Task1);
            Assert.AreEqual("Start To Task 1", sequenceFlow_StartEvent_Task1.Name);
            Assert.AreEqual("startevent_1", sequenceFlow_StartEvent_Task1.SourceRef);
            Assert.AreEqual("test_task_1", sequenceFlow_StartEvent_Task1.TargetRef);
        }
    }
}
