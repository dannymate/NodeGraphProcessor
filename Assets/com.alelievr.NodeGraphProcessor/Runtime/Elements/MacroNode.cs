using System;
using GraphProcessor;

[System.Serializable]
public class MacroNode : SubGraphNode
{
    public override bool needsInspector => false;

    public void SetMacro(SubGraph macro)
    {
        this.subGraph = macro;
    }
}