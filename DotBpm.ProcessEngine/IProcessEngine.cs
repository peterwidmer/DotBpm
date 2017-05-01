using DotBpm.Bpmn.BpmnModel;
using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBom.Engines
{
    public interface IProcessEngine
    {
        ProcessInstance GetProcessInstance(string processName);
    }
}
