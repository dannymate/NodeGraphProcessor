using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[System.Serializable, NodeMenuItem("Subgraph")]
public class SubGraphNode : BaseNode
{
    public const string IngressPortsField = nameof(_ingress);
    public const string EgressPortsField = nameof(_egress);
    public const string SubGraphField = nameof(subGraph);

    [SerializeField]
    protected SubGraph subGraph;

    [Input, CustomBehaviourOnly]
    protected object _ingress;

    [Output, CustomBehaviourOnly]
    protected object _egress;

    protected Dictionary<PortData, object> _passThroughBufferByPort = new();

    protected override NodeRenamePolicy DefaultRenamePolicy => SubGraph?.Options.RenamePolicy ?? NodeRenamePolicy.DISABLED;
    public override bool HideNodeInspectorBlock => true;
    public override bool needsInspector => true;
    public override string name
    {
        get
        {
            if (!SubGraph)
                return "SubGraphNode";

            if (string.IsNullOrWhiteSpace(SubGraph.Options.DisplayName))
                return SubGraph.name;

            return SubGraph.Options.DisplayName;
        }
    }
    public override Color AccentColor => new Color(1, 0, 1, 0.5f);

    public SubGraph SubGraph => subGraph;

    protected List<PortData> IngressPortData => SubGraph?.IngressPortData ?? new List<PortData>();
    protected List<PortData> EgressPortData => SubGraph?.EgressPortData ?? new List<PortData>();

    public override void InitializePorts()
    {
        base.InitializePorts();

        _passThroughBufferByPort?.Clear();
        SubGraph?.AddUpdatePortsListener(OnPortsListUpdated);
        SubGraph?.AddOptionsListener(OnSubGraphOptionsChanged);
    }

    protected override void Process()
    {
        base.Process();

        var processor = new ProcessSubGraphProcessor(SubGraph);
        processor.Run(_passThroughBufferByPort);
    }

    [CustomPortInput(nameof(_ingress), typeof(object))]
    protected void PullIngress(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = IngressPortData.Find(x => x.Equals(connectedEdges[0].inputPort.portData));
        _passThroughBufferByPort[portData] = connectedEdges[0].passThroughBuffer;
    }

    [CustomPortBehavior(nameof(_ingress), cloneResults: true)]
    protected IEnumerable<PortData> CreateIngressPorts(List<SerializableEdge> edges)
    {
        if (IngressPortData == null) yield break;

        foreach (var input in IngressPortData)
        {
            if (string.IsNullOrEmpty(input.Identifier))
                input.identifier = input.displayName;

            yield return input;
        }
    }

    [CustomPortInput(nameof(_egress), typeof(object))]
    protected void PushEgress(List<SerializableEdge> connectedEdges)
    {
        if (connectedEdges.Count == 0) return;

        PortData portData = EgressPortData.Find(x => x.Equals(connectedEdges[0].outputPort.portData));
        Dictionary<PortData, object> returnedData = SubGraph.EgressNode.PushEgress();

        foreach (var edge in connectedEdges)
        {
            if (returnedData.ContainsKey(portData))
                edge.passThroughBuffer = returnedData[portData];
        }
    }

    [CustomPortBehavior(nameof(_egress), cloneResults: true)]
    protected IEnumerable<PortData> CreateEgressPorts(List<SerializableEdge> edges)
    {
        if (EgressPortData == null) yield break;

        foreach (var output in EgressPortData)
        {
            if (String.IsNullOrEmpty(output.Identifier))
                output.identifier = output.displayName;

            yield return output;
        }
    }

    private void OnPortsListUpdated()
    {
        UpdateAllPortsLocal();
    }

    private void OnSubGraphOptionsChanged()
    {
        RepaintTitle();
    }
}