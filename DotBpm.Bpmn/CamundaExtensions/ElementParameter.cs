using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.CamundaExtensions
{
    public class ElementParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ElementParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
