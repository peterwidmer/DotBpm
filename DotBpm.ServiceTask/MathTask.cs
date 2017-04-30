using DotBpm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.ServiceTask
{
    public class MathTask : IServiceTask
    {
        private const string RESULTVARIABLE = "result";

        public void Execute(ServiceTaskContext serviceTaskContext)
        {
            string operation = serviceTaskContext.GetVariable<string>("operation");
            int parameter1 = serviceTaskContext.GetVariable<int>("parameter1");
            int parameter2 = serviceTaskContext.GetVariable<int>("parameter2");

            switch(operation)
            {
                case "add":
                    serviceTaskContext.SetVariable(RESULTVARIABLE, parameter1 + parameter2);
                    break;

                case "subtract":
                    serviceTaskContext.SetVariable(RESULTVARIABLE, parameter1 - parameter2);
                    break;
            }
        }
    }
}
