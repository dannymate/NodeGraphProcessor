using System.Collections.Generic;

namespace GraphProcessor
{
    [System.Serializable]
    public class SubGraphIngressNode : SubGraphBoundaryNode
    {
        [Output, CustomBehaviourOnly]
        private object _ingress;

        public override string name => "Ingress";
        protected override List<PortData> Ports => SubGraph.IngressPortData;

        public void PullIngress(Dictionary<PortData, object> ingress)
        {
            passThroughBufferByPort = ingress;
        }

        protected override void PostProcess()
        {
            passThroughBufferByPort.Clear();
        }

        [CustomPortOutput(nameof(_ingress), typeof(object))]
        protected void PushIngress(List<SerializableEdge> connectedEdges)
        {
            if (connectedEdges.Count == 0 || passThroughBufferByPort.Count == 0) return;

            var value = passThroughBufferByPort[connectedEdges[0].outputPort.portData];
            foreach (var edge in connectedEdges)
            {
                edge.passThroughBuffer = value;
            }
        }

        [CustomPortBehavior(nameof(_ingress), cloneResults: true)]
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