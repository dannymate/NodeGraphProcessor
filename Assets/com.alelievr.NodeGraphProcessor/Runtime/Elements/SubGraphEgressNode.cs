using System.Collections.Generic;

namespace GraphProcessor
{
    [System.Serializable]
    public class SubGraphEgressNode : SubGraphBoundaryNode
    {
        [Input, CustomBehaviourOnly]
        private object _egress;

        public override string name => "Egress";
        protected override List<PortData> Ports => SubGraph.EgressPortData;

        public Dictionary<PortData, object> PushEgress()
        {
            return passThroughBufferByPort;
        }

        protected override void PreProcess()
        {
            passThroughBufferByPort.Clear();
        }

        [CustomPortInput(nameof(_egress), typeof(object))]
        protected void PullEgressPorts(List<SerializableEdge> connectedEdges)
        {
            if (connectedEdges.Count == 0) return;

            passThroughBufferByPort.Add(connectedEdges[0].inputPort.portData, connectedEdges[0].passThroughBuffer);
        }

        [CustomPortBehavior(nameof(_egress), cloneResults: true)]
        protected IEnumerable<PortData> CreatePorts(List<SerializableEdge> edges)
        {
            if (Ports == null) yield break;

            foreach (var portData in Ports)
            {
                yield return portData;
            }
        }
    }
}