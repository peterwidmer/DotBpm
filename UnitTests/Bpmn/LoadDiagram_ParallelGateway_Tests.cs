using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using DotBpm.Bpmn;
using DotBpm.Bpmn.BpmnModel;
using System.Linq;

namespace UnitTests.Bpmn
{
    /// <summary>
    /// Summary description for LoadDiagram_ParallelGateway_Test
    /// </summary>
    [TestClass]
    public class LoadDiagram_ParallelGateway_Tests
    {
        [TestMethod]
        public void LoadDiagram_ParallelGateway_Test()
        {
            var process = BpmnDiagram_TestHelper.LoadDiagram("UnitTests.Bpmn.diagram_parallelgateway.bpmn");

            var parallelGateway1 = (BpmnParallelGateway)process.Artifacts.FirstOrDefault(t => t.Id == "parallel_gateway_1");
            Assert.AreEqual("parallel_gateway_1", parallelGateway1.Id);
            Assert.AreEqual("Parallelgateway 1", parallelGateway1.Name);
            Assert.AreEqual(1, parallelGateway1.Incoming.Count);
            Assert.AreEqual(2, parallelGateway1.Outgoing.Count);
        }
    }
}
