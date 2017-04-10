using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Sdk
{
    public interface IServiceTask
    {
        void Execute(ServiceTaskContext serviceTaskContext);
    }
}
