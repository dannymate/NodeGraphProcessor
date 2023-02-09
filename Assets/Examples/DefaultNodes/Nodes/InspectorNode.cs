using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using UnityEngine.UIElements;

[System.Serializable, NodeMenuItem("Custom/InspectorNode")]
public class InspectorNode : BaseNode
{
    [Input(name = "In")]
    public float input;

    [Output(name = "Out")]
    public float output;

    [ShowInInspector, SerializeField]
    public bool additionalSettings;
    [ShowInInspector]
    public string additionalParam;

    public override string name => "InspectorNode";
    public override bool needsInspector => true;

    protected override void Process()
    {
        output = input * 42;
    }
}
