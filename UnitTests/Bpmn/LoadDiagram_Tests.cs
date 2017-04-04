﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Bpmn;
using System.Linq;

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

            var startEvent1 = process.StartEvents.FirstOrDefault(t => t.Id == "startevent_1");
            Assert.IsNotNull(startEvent1);
            Assert.AreEqual("Start Event 1", startEvent1.Name);

            var task1 = process.Tasks.FirstOrDefault(t => t.Id == "test_task_1");
            Assert.IsNotNull(task1);
            Assert.AreEqual("Test Task 1", task1.Name);

            var sequenceFlow_StartEvent_Task1 = process.SequenceFlows.FirstOrDefault(t => t.Id == "start_task_1");
            Assert.IsNotNull(sequenceFlow_StartEvent_Task1);
            Assert.AreEqual("Start To Task 1", sequenceFlow_StartEvent_Task1.Name);
            Assert.AreEqual("startevent_1", sequenceFlow_StartEvent_Task1.SourceRef);
            Assert.AreEqual("test_task_1", sequenceFlow_StartEvent_Task1.TargetRef);
        }
    }
}