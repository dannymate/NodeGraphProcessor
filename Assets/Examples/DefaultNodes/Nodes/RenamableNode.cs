using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Custom/Renamable")]
public class RenamableNode : BaseNode
{
    [Output("Out")]
    public float output;

    [Input("In")]
    public float input;

    public override string name => "Renamable";

    protected override NodeRenamePolicy DefaultRenamePolicy => NodeRenamePolicy.BOTH;

    protected override void Process() => output = input;
}