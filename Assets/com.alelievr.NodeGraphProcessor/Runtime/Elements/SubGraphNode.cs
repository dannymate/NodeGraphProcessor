using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using TypeReferences;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;

[System.Serializable, NodeMenuItem("Subgraph")]
public class SubGraphNode : BaseNode
{
    [Input("Inputs")]
    [CustomBehaviourOnly]
    object inputs;

    [Output]
    [CustomBehaviourOnly]
    object outputs;

    [SerializeField]
    SubGraph subGraph;

    Dictionary<PortData, object> passThroughBuffer = new();

    private List<PortData> InputData => subGraph?.InputData;
    private List<PortData> OutputData => subGraph?.OutputData;
    public override bool HideNodeInspectorBlock => true;
    public override bool needsInspector => true;

    protected override void Process()
    {
        base.Process();

        var processor = new ProcessSubGraphProcessor(subGraph);
        processor.Run(passThroughBuffer);
    }

    public override void InitializePorts()
    {
        base.InitializePorts();

        passThroughBuffer?.Clear();
        subGraph?.AddUpdatePortsListener(OnPortsListUpdated);
    }


    [CustomPortInput(nameof(inputs), typeof(object))]
    protected void PullInputs(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = InputData.Find(x => x.Equals(connectedEdges[0].inputPort.portData));
        passThroughBuffer[portData] = connectedEdges[0].passThroughBuffer;
    }

    [CustomPortBehavior(nameof(inputs), cloneResults: true)]
    protected IEnumerable<PortData> CreateInputs(List<SerializableEdge> edges)
    {
        if (InputData == null) yield break;

        foreach (var input in InputData)
        {
            if (String.IsNullOrEmpty(input.identifier))
                input.identifier = input.displayName;

            yield return input;
        }
    }

    [CustomPortInput(nameof(outputs), typeof(object))]
    protected void PushOutputs(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = OutputData.Find(x => x.Equals(connectedEdges[0].outputPort.portData));
        Dictionary<PortData, object> returnedData = subGraph.ReturnNode.GetReturnValue();
        foreach (var edge in connectedEdges)
        {
            if (returnedData.ContainsKey(portData))
                edge.passThroughBuffer = returnedData[portData];
        }
    }

    [CustomPortBehavior(nameof(outputs), cloneResults: true)]
    protected IEnumerable<PortData> CreateOutputs(List<SerializableEdge> edges)
    {
        if (OutputData == null) yield break;

        foreach (var output in OutputData)
        {
            if (String.IsNullOrEmpty(output.identifier))
                output.identifier = output.displayName;

            yield return output;
        }
    }

    public override void DrawControlsContainer(VisualElement root)
    {
        base.DrawControlsContainer(root);

        if (subGraph == null) return;

        subGraph.CreateInspectorGUI(root);
    }

    public void OnPortsListUpdated() => UpdateAllPortsLocal();
}