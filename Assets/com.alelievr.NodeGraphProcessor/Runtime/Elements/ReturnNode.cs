using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using TypeReferences;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GraphProcessor
{
    [System.Serializable]
    public class ReturnNode : BaseNode
    {
        [Input]
        [CustomBehaviourOnly]
        object inputs;

        Dictionary<PortData, object> outputDict = new();

        SubGraph SubGraph => graph as SubGraph;
        private List<PortData> ReturnPorts => SubGraph.outputData;

        public override bool deletable => true;
        public override bool needsInspector => true;
        public override bool HideNodeInspectorBlock => true;

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

            foreach (var portData in ReturnPorts) // Doesn't work if we have multiple of the same type
            {
                yield return portData;
            }
        }

        public Dictionary<PortData, object> GetReturnValue()
        {
            return outputDict;
        }

        public override void DrawControlsContainer(VisualElement root)
        {
            base.DrawControlsContainer(root);

            var outputsField = new PropertyField(SubGraph.OutputDataSerialized);
            var updateButton = new Button(() => SubGraph.NotifyPortsChanged()) { text = "UPDATE PORTS" };

            outputsField.Bind(SubGraph.ThisSerialized);

            root.Add(outputsField);
            root.Add(updateButton);
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

        Dictionary<PortData, object> passThroughBuffer = new();

        SubGraph SubGraph => graph as SubGraph;
        private List<PortData> IngressPorts => SubGraph.inputData;

        public override bool deletable => true;
        public override bool needsInspector => true;
        public override bool HideNodeInspectorBlock => true;

        public override void InitializePorts()
        {
            base.InitializePorts();
            SubGraph.AddUpdatePortsListener(OnPortsListUpdated);
            passThroughBuffer.Clear();
        }
        public void OnPortsListUpdated() => UpdateAllPortsLocal();

        public void PullIngress(Dictionary<PortData, object> ingress)
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

            foreach (var portData in IngressPorts) // Doesn't work if we have multiple of the same type
            {
                yield return portData;
            }
        }

        public override void DrawControlsContainer(VisualElement root)
        {
            base.DrawControlsContainer(root);

            var inputsField = new PropertyField(SubGraph.InputDataSerialized);
            var updateButton = new Button(() => SubGraph.NotifyPortsChanged()) { text = "UPDATE PORTS" };

            inputsField.Bind(SubGraph.ThisSerialized);

            root.Add(inputsField);
            root.Add(updateButton);
        }
    }
}