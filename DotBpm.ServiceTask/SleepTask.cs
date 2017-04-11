using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DotBpm.ServiceTask
{
    public class SleepTask : IServiceTask
    {
        public void Execute(ServiceTaskContext serviceTaskContext)
        {
            Console.WriteLine("Start " + serviceTaskContext.Token.CurrentElementId);
            for(int i=0; i < 5; i++)
            {
                Trace.WriteLine(serviceTaskContext.Token.CurrentElementId + i);
                Thread.Sleep(500);

            }
            Trace.WriteLine("Finished " + serviceTaskContext.Token.CurrentElementId);
        }
    }
}
