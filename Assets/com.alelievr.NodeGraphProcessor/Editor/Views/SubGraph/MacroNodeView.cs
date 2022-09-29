using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(MacroNode))]
    public class MacroNodeView : BaseNodeView
    {
        MacroNode Target => nodeTarget as MacroNode;
        SubGraph SubGraph => Target.SubGraph;

        SubGraphSerializerUtility SubGraphSerializer => SubGraph ? new(SubGraph) : null;

        protected override void DrawDefaultInspector(bool fromInspector = false) { }
    }
}