using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBpm.Bpmn.BpmnModel
{
    public class BpmnProcess : BpmnBaseElement
    {
        public bool IsExecutable { get; set; }

        public List<BpmnBaseElement> Artifacts { get; set; }

        public Dictionary<string, BpmnBaseElement> ArtifactIndex { get; set; }

        public BpmnProcess()
        {
            Artifacts = new List<BpmnBaseElement>();
            ArtifactIndex = new Dictionary<string, BpmnBaseElement>();
        }

        public void IndexArtifacts()
        {
            ArtifactIndex.Clear();
            IndexLocalArtifact(Artifacts);
        }

        private void IndexLocalArtifact(List<BpmnBaseElement> artifacts)
        {
            foreach(var artifact in artifacts)
            {
                ArtifactIndex.Add(artifact.Id, artifact);
                if(artifact is BpmnSubProcess)
                {
                    IndexLocalArtifact(((BpmnSubProcess)artifact).Artifacts);
                }
            }
        }
    }
}
