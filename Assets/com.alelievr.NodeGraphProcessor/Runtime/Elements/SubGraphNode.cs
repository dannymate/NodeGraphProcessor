using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[System.Serializable, NodeMenuItem("Subgraph")]
public class SubGraphNode : BaseNode
{
    [Input, CustomBehaviourOnly]
    private object _inputs;

    [Output, CustomBehaviourOnly]
    private object _outputs;

    [SerializeField]
    private SubGraph subGraph;

    private Dictionary<PortData, object> passThroughBufferByPort = new();

    public override bool HideNodeInspectorBlock => true;
    public override bool needsInspector => true;

    private List<PortData> InputData => subGraph != null ? subGraph.InputData : new List<PortData>();
    private List<PortData> OutputData => subGraph != null ? subGraph.OutputData : new List<PortData>();

    public override void InitializePorts()
    {
        base.InitializePorts();

        passThroughBufferByPort?.Clear();
        subGraph?.AddUpdatePortsListener(OnPortsListUpdated);
    }

    public override void DrawControlsContainer(VisualElement root)
    {
        base.DrawControlsContainer(root);

        if (subGraph == null) return;

        subGraph.CreateInspectorGUI(root);
    }

    protected override void Process()
    {
        base.Process();

        var processor = new ProcessSubGraphProcessor(subGraph);
        processor.Run(passThroughBufferByPort);
    }

    [CustomPortInput(nameof(_inputs), typeof(object))]
    protected void PullIngress(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = InputData.Find(x => x.Equals(connectedEdges[0].inputPort.portData));
        passThroughBufferByPort[portData] = connectedEdges[0].passThroughBuffer;
    }

    [CustomPortBehavior(nameof(_inputs), cloneResults: true)]
    protected IEnumerable<PortData> CreateIngressPorts(List<SerializableEdge> edges)
    {
        if (InputData == null) yield break;

        foreach (var input in InputData)
        {
            if (String.IsNullOrEmpty(input.identifier))
                input.identifier = input.displayName;

            yield return input;
        }
    }

    [CustomPortInput(nameof(_outputs), typeof(object))]
    protected void PushEgress(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = OutputData.Find(x => x.Equals(connectedEdges[0].outputPort.portData));
        Dictionary<PortData, object> returnedData = subGraph.EgressNode.PushEgress();
        foreach (var edge in connectedEdges)
        {
            if (returnedData.ContainsKey(portData))
                edge.passThroughBuffer = returnedData[portData];
        }
    }

    [CustomPortBehavior(nameof(_outputs), cloneResults: true)]
    protected IEnumerable<PortData> CreateEgressPorts(List<SerializableEdge> edges)
    {
        if (OutputData == null) yield break;

        foreach (var output in OutputData)
        {
            if (String.IsNullOrEmpty(output.identifier))
                output.identifier = output.displayName;

            yield return output;
        }
    }

    private void OnPortsListUpdated() => UpdateAllPortsLocal();
}