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

        SubGraphSerializerUtility SubGraphSerializer => SubGraph ? new(SubGraph) : null;

        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            controlsContainer.Add(SubGraphSerializer?.DrawFullSubGraphGUI());
            controlsContainer.Add(DrawSchemaControls());

            base.DrawDefaultInspector(fromInspector);
        }

        protected VisualElement DrawSchemaControls()
        {
            VisualElement schemaControls = new();
            PropertyField schemaField = SubGraphSerializer.DrawSchemaFieldWithCallback((prop) =>
            {
                // We sanity check visibility due to this callback being called twice
                if (schemaControls.visible && SubGraph.Schema == null)
                {
                    schemaControls.visible = false;
                    schemaControls.Clear();
                }
                else if (!schemaControls.visible && SubGraph.Schema != null)
                {
                    schemaControls.Add(SubGraphSerializer.SchemaSerializer.DrawFullSchemaGUI());
                    schemaControls.visible = true;
                }
            }, visible: false);
            schemaControls.Add(schemaField);
            schemaControls.Add(SubGraphSerializer.SchemaSerializer?.DrawFullSchemaGUI());

            return schemaControls;
        }
    }
}