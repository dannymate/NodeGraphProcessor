using System;
using GraphProcessor;
using UnityEngine;

[System.Serializable]
public class MacroNode : SubGraphNode
{
    public override bool needsInspector => false;
    public override Color color => Color.blue;

    public void SetMacro(SubGraph macro)
    {
        this.subGraph = macro;
    }

    public static BaseNode InstantiateMacro(Type nodeType, Vector2 position, params object[] args)
    {
        SubGraph macro = args[0] as SubGraph;
        MacroNode macroNode = BaseNode.CreateFromType(nodeType, position, args) as MacroNode;
        macroNode.SetMacro(macro);
        return macroNode;
    }
}