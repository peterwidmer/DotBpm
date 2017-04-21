using DotBpm.Bpmn;
using DotBpm.Bpmn.BpmnModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace UnitTests.Bpmn
{
    public class BpmnDiagram_TestHelper
    {
        public static BpmnProcess LoadDiagram(string diagramName)
        {
            var bpmnDocument = XDocument.Parse(TestHelper.GetEmbeddedResourceAsString(diagramName));

            var bpmnParser = new BpmnParser(bpmnDocument);
            return bpmnParser.Parse();
        }
    }
}
