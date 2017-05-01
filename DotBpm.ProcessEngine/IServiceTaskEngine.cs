using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Engines
{
    public interface IServiceTaskEngine
    {
        IServiceTask GetInstance(string className);
    }
}
