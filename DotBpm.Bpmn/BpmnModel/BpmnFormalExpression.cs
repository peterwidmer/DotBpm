using System;
using System.Collections.Generic;
using System.Text;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnFormalExpression : BpmnExpression
    {
        public string Language { get; set; }
        public string Body { get; set; }
    }
}
