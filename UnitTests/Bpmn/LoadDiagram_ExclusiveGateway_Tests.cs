using DotBpm.Bpmn;
using DotBpm.Bpmn.BpmnModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.Bpmn
{
    /// <summary>
    /// Summary description for LoadDiagram_ParallelGateway_Test
    /// </summary>
    [TestClass]
    public class LoadDiagram_ExclusiveGateway_Tests
    {
        [TestMethod]
        public void LoadDiagram_ExclusiveGateway_Test()
        {
            var bpmnDocument = new XmlDocument();
            bpmnDocument.LoadXml(TestHelper.GetEmbeddedResourceAsString("UnitTests.Bpmn.diagram_exclusivegateway.bpmn"));

            var bpmnParser = new BpmnParser(bpmnDocument);
            var process = bpmnParser.Parse();

            var exclusiveGateway1 = (BpmnExclusiveGateway)process.Elements.FirstOrDefault(t => t.Id == "exclusive_gateway_1");
            Assert.AreEqual("exclusive_gateway_1", exclusiveGateway1.Id);
            Assert.AreEqual("Exclusivegateway 1", exclusiveGateway1.Name);
            Assert.AreEqual(1, exclusiveGateway1.Incoming.Count);
            Assert.AreEqual(2, exclusiveGateway1.Outgoing.Count);

            var seq_parallelgateway_1_test_task_2 = (BpmnSequenceFlow)process.Elements.FirstOrDefault(t => t.Id == "seq_parallelgateway_1_test_task_2");
            Assert.AreEqual("javascript", seq_parallelgateway_1_test_task_2.ConditionExpression.Language);
            Assert.AreEqual("return '${decision}' == 'test_task_2';", seq_parallelgateway_1_test_task_2.ConditionExpression.Body);
        }
    }
}
