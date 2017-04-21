using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DotBpm.Bpmn
{
    public class BpmnXmlHelper
    {
        public static bool GetAttributeBoolean(XElement xmlElement, string attributeName)
        {
            string value = xmlElement.Attribute(attributeName)?.Value;
            if(!string.IsNullOrEmpty(value))
            {
                return Convert.ToBoolean(value);
            }

            return false;
        }
    }
}
