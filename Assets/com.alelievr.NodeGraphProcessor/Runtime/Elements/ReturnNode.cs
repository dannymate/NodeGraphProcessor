using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using TypeReferences;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [System.Serializable]
    public class ReturnNode : BaseNode
    {
        [Input]
        [CustomBehaviourOnly]
        object inputs;

        Dictionary<PortDataRef, object> outputDict = new();

        SubGraph SubGraph => graph as SubGraph;
        private List<PortDataRef> ReturnPorts => SubGraph.outputData;

        public override bool deletable => true;

        public override void InitializePorts()
        {
            base.InitializePorts();
            SubGraph.AddUpdatePortsListener(OnPortsListUpdated);
        }
        public void OnPortsListUpdated() => UpdateAllPortsLocal();


        [CustomPortInput(nameof(inputs), typeof(object))]
        protected void PullInputs(List<SerializableEdge> connectedEdges)
        {
            outputDict.Clear();

            if (connectedEdges.Count == 0) return;

            outputDict.Add(ReturnPorts.Find(x => x.Equals(connectedEdges[0].inputPort.portData)), connectedEdges[0].passThroughBuffer);
        }

        [CustomPortBehavior(nameof(inputs))]
        protected IEnumerable<PortData> CreateInputs(List<SerializableEdge> edges)
        {
            if (ReturnPorts == null) yield break;

            foreach (var port in ReturnPorts) // Doesn't work if we have multiple of the same type
            {
                yield return new PortData
                {
                    identifier = ReturnPorts.IndexOf(port).ToString(),
                    displayName = port.Label,
                    displayType = port.DisplayType,
                    vertical = port.Vertical,
                    acceptMultipleEdges = port.AcceptMultipleEdges,
                    showAsDrawer = port.ShowAsDrawer
                };
            }
        }

        public Dictionary<PortDataRef, object> GetReturnValue()
        {
            return outputDict;
        }

        public override void DrawControlsContainer(VisualElement root)
        {
            base.DrawControlsContainer(root);
        }

    }
}

namespace GraphProcessor
{
    [System.Serializable]
    public class IngressNode : BaseNode
    {
        [Output]
        [CustomBehaviourOnly]
        object outputs;

        Dictionary<PortDataRef, object> passThroughBuffer = new();

        SubGraph SubGraph => graph as SubGraph;
        private List<PortDataRef> IngressPorts => SubGraph.inputData;

        public override bool deletable => true;

        public override void InitializePorts()
        {
            base.InitializePorts();
            SubGraph.AddUpdatePortsListener(OnPortsListUpdated);
            passThroughBuffer.Clear();
        }
        public void OnPortsListUpdated() => UpdateAllPortsLocal();

        public void PullIngress(Dictionary<PortDataRef, object> ingress)
        {
            this.passThroughBuffer = ingress;
        }

        [CustomPortOutput(nameof(outputs), typeof(object))]
        protected void PushOutputs(List<SerializableEdge> connectedEdges)
        {
            if (connectedEdges.Count == 0) return;

            var value = passThroughBuffer[IngressPorts.Find(x => x.Equals(connectedEdges[0].outputPort.portData))];
            foreach (var edge in connectedEdges)
            {
                edge.passThroughBuffer = value;
            }
        }

        [CustomPortBehavior(nameof(outputs))]
        protected IEnumerable<PortData> CreateOutputs(List<SerializableEdge> edges)
        {
            if (IngressPorts == null) yield break;

            foreach (var port in IngressPorts) // Doesn't work if we have multiple of the same type
            {
                yield return new PortData
                {
                    identifier = IngressPorts.IndexOf(port).ToString(),
                    displayName = port.Label,
                    displayType = port.DisplayType,
                    vertical = port.Vertical,
                    acceptMultipleEdges = port.AcceptMultipleEdges,
                    showAsDrawer = port.ShowAsDrawer
                };
            }
        }

        public override void DrawControlsContainer(VisualElement root)
        {
            base.DrawControlsContainer(root);
        }
    }
}