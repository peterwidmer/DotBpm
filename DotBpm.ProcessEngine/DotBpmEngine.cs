using DotBpm.Sdk;
using DotBpm.StorageEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Engines
{
    public class DotBpmEngine
    {
        IExecutionScopeStore executionScopeStore;
        IServiceTaskEngine serviceTaskEngine;

        public DotBpmEngine(IExecutionScopeStore executionScopeStore, IServiceTaskEngine serviceTaskEngine)
        {
            this.executionScopeStore = executionScopeStore;
            this.serviceTaskEngine = serviceTaskEngine;
        }

        public void ExecuteProcess(ProcessInstance processInstance)
        {
            ExecuteProcess(processInstance, new Dictionary<string, object>());
        }

        public void ExecuteProcess(ProcessInstance processInstance, Dictionary<string, object> variables)
        {
            var runEngine = new RunEngine(processInstance, executionScopeStore, serviceTaskEngine);
            var task = runEngine.ExecuteProcess(variables);
            task.Wait();
        }
    }
}
