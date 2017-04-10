using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public class ServiceTaskContext
    {
        public ProcessToken Token { get; }

        public ServiceTaskContext(ProcessToken token)
        {
            Token = token;
        }
        
    }
}
