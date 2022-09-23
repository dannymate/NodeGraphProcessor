using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using TypeReferences;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

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

    Dictionary<PortDataRef, object> passThroughBuffer = new();

    private List<PortDataRef> InputData => subGraph?.inputData;
    private List<PortDataRef> OutputData => subGraph?.outputData;
    public override bool HideNodeInspectorBlock => true;
    public override bool needsInspector => true;

    protected override void Process()
    {
        base.Process();

        var processor = new ProcessSubGraphProcessor(subGraph);
        processor.Run(passThroughBuffer);
        Debug.Log(passThroughBuffer.Count);
    }

    public override void InitializePorts()
    {
        base.InitializePorts();

        passThroughBuffer?.Clear();

        if (subGraph)
            subGraph.AddUpdatePortsListener(OnPortsListUpdated);
    }


    [CustomPortInput(nameof(inputs), typeof(object))]
    protected void PullInputs(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        var portDataRef = InputData.Find(x => x.Equals(connectedEdges[0].inputPort.portData));
        passThroughBuffer[portDataRef] = connectedEdges[0].passThroughBuffer;
    }

    [CustomPortBehavior(nameof(inputs))]
    protected IEnumerable<PortData> CreateInputs(List<SerializableEdge> edges)
    {
        if (InputData == null) yield break;

        foreach (var input in InputData) // Doesn't work if we have multiple of the same type
        {
            yield return new PortData
            {
                identifier = InputData.IndexOf(input).ToString(),
                displayName = input.Label,
                displayType = input.DisplayType,
                vertical = input.Vertical,
                acceptMultipleEdges = input.AcceptMultipleEdges,
                showAsDrawer = input.ShowAsDrawer
            };
        }
    }

    [CustomPortInput(nameof(outputs), typeof(object))]
    protected void PushOutputs(List<SerializableEdge> connectedEdges)
    {
        foreach (var edge in connectedEdges)
        {
            edge.passThroughBuffer = subGraph.ReturnNode.GetReturnValue()[OutputData.Find(x => x.Equals(edge.outputPort.portData))];
        }
    }

    [CustomPortBehavior(nameof(outputs))]
    protected IEnumerable<PortData> CreateOutputs(List<SerializableEdge> edges)
    {
        if (OutputData == null) yield break;

        foreach (var output in OutputData) // Doesn't work if we have multiple of the same type
        {
            yield return new PortData
            {
                identifier = OutputData.IndexOf(output).ToString(),
                displayName = output.Label,
                displayType = output.DisplayType,
                vertical = output.Vertical,
                acceptMultipleEdges = output.AcceptMultipleEdges,
                showAsDrawer = output.ShowAsDrawer
            };
        }
    }

    public override void DrawControlsContainer(VisualElement root)
    {
        base.DrawControlsContainer(root);

        if (subGraph == null) return;

        subGraph.DrawPortSelectionGUI(root);
    }

    public void OnPortsListUpdated() => UpdateAllPortsLocal();
}