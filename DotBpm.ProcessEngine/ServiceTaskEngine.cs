using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using DotBpm.Sdk;

namespace DotBpm.ProcessEngine
{
    public class ServiceTaskEngine
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

        public object GetInstance(string className)
        {
            Type serviceTaskType = AvailablServiceTasks.FirstOrDefault(t => t.Name == className);
            if (serviceTaskType != null)
            {
                return Activator.CreateInstance(serviceTaskType);
            }
            else
            {
                throw new ArgumentOutOfRangeException(className, "No servicetask of type " + className + " was found.");
            }
        }
    }
}
