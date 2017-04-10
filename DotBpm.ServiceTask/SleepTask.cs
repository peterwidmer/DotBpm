using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotBpm.ServiceTask
{
    public class SleepTask : IServiceTask
    {
        public void Execute(ServiceTaskContext serviceTaskContext)
        {
            Console.WriteLine("Start " + serviceTaskContext.Token.CurrentElementId);
            Thread.Sleep(1000);
            Console.WriteLine("Finished " + serviceTaskContext.Token.CurrentElementId);
        }
    }
}
