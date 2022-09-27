using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(SubGraphNode))]
    public class SubGraphNodeView : BaseNodeView
    {
        SubGraphNode Target => nodeTarget as SubGraphNode;
        SubGraph SubGraph => Target.SubGraph;

        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            controlsContainer.Add(DrawSubGraphControlsGUI());
            base.DrawDefaultInspector(fromInspector);
        }

        public VisualElement DrawSubGraphControlsGUI()
        {
            if (SubGraph == null) return new VisualElement();

            SubGraph.CreateInspectorGUI(controlsContainer);

            return new VisualElement();
        }
    }
}