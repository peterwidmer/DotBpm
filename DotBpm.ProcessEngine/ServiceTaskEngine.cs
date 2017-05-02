using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using DotBpm.Sdk;
using DotBpm.Engines;

namespace DotBpm.Engines
{
    public class ServiceTaskEngine : IServiceTaskEngine
    {
        private List<Type> AvailablServiceTasks = new List<Type>();

        public ServiceTaskEngine()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var serviceTaks = assembly
                    .GetTypes()
                    .Where(x => x.GetInterface(typeof(IServiceTask).Name) != null)
                    .Select(x => x)
                    .ToList();

                AvailablServiceTasks.AddRange(serviceTaks);
            }
        }

        public IServiceTask GetInstance(string className)
        {
            Type serviceTaskType = AvailablServiceTasks.FirstOrDefault(t => t.Name == className || t.FullName == className);
            if (serviceTaskType != null)
            {
                return (IServiceTask)Activator.CreateInstance(serviceTaskType);
            }
            else
            {
                throw new ArgumentOutOfRangeException(className, "No servicetask of type " + className + " was found.");
            }
        }
    }
}
