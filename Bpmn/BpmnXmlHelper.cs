using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bpmn
{
    public class BpmnXmlHelper
    {
        public static bool GetAttributeBoolean(XmlElement xmlElement, string attributeName)
        {
            string value = xmlElement.GetAttribute(attributeName);
            if(!string.IsNullOrEmpty(value))
            {
                return Convert.ToBoolean(value);
            }

            return false;
        }
    }
}
