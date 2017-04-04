﻿using Bpmn.BpmnModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpmSdk
{
    public class ProcessInstance
    {
        public Guid Id { get; set; }
        public BpmnProcess ProcessDefinition { get; set; }
        public List<ProcessToken> Tokens { get; set; }

        public ProcessInstance()
        {
            Tokens = new List<ProcessToken>();
        }
    }
}