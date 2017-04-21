﻿using DotBpm.Bpmn.BpmnModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Bpmn
{
    [TestClass]
    public class LoadDiagram_SubProcess_Tests
    {
        [TestMethod]
        public void LoadDiagram_SubProcess_Test()
        {
            var process = BpmnDiagram_TestHelper.LoadDiagram("UnitTests.Bpmn.diagram_subprocess.bpmn");

            var subProcess1 = process.Artifacts.FirstOrDefault(t=> t.Id == "sub_process_1") as BpmnSubProcess;
            Assert.IsNotNull(subProcess1);

            var subTask1 = subProcess1.Artifacts.FirstOrDefault(t => t.Id == "sub_task_1");
            Assert.IsNotNull(subTask1);

            var subsubProcess1 = subProcess1.Artifacts.FirstOrDefault(t => t.Id == "sub_sub_process_1") as BpmnSubProcess;
            Assert.IsNotNull(subsubProcess1);

            var subsubStartEvent = subsubProcess1.Artifacts.FirstOrDefault(t => t.Id == "sub_sub_start_event");
            Assert.IsNotNull(subsubStartEvent);

            var subsubTask2 = subsubProcess1.Artifacts.FirstOrDefault(t => t.Id == "sub_sub_task_2");
            Assert.IsNotNull(subsubTask2);
        }
    }
}
