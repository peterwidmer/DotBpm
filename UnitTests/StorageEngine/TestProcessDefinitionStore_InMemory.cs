using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestProcessDefinitionStore_InMemory
    {
        [TestMethod]
        public void ProcessDefinitionStore_Test()
        {
            string bpmnContentV1 = "This would be the first process";
            string bpmnContentV2 = "This would be the second process";

            var processDefinitionStore = new ProcessDefinitionStore_InMemory();
            processDefinitionStore.Save("my process", bpmnContentV1);
            processDefinitionStore.Save("my process", bpmnContentV2);

            var myProcess_test1 = processDefinitionStore.Load("my process");
            Assert.AreEqual(2, myProcess_test1.Version);
            Assert.AreEqual(bpmnContentV2, myProcess_test1.BpmnContent);

            var myProcess_test2 = processDefinitionStore.Load("my process", 1);
            Assert.AreEqual(1, myProcess_test2.Version);
            Assert.AreEqual(bpmnContentV1, myProcess_test2.BpmnContent);
        }
    }
}
