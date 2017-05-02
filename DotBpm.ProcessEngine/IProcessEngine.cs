using DotBpm.Bpmn.BpmnModel;
using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Engines
{
    public interface IProcessEngine
    {
        ProcessInstance GetProcessInstance(string processName);
    }
}
