using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(SubGraphEgressNode))]
    public class SubgraphEgressNodeView : BaseNodeView
    {
        SubGraphEgressNode Target => this.nodeTarget as SubGraphEgressNode;
        SubGraph SubGraph => Target.SubGraph;

        SubGraphSerializerUtility SubGraphSerializer => SubGraph ? new(SubGraph) : null;


        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            controlsContainer.Add(DrawSubGraphControlsGUI());
            controlsContainer.Add(DrawSchemaControlsGUI());
        }

        public VisualElement DrawSubGraphControlsGUI()
        {
            var subgraphPortFoldout = new Foldout()
            {
                text = "Local SubGraph Port Selection"
            };

            subgraphPortFoldout.Add(SubGraphSerializer.DrawEgressPortSelectorGUI());
            subgraphPortFoldout.Add(SubGraphSerializer.DrawPortUpdaterButtonGUI());

            return subgraphPortFoldout;

        }

        public VisualElement DrawSchemaControlsGUI()
        {
            var schemaFoldout = new Foldout()
            {
                text = "Schema Port Selection"
            };
            VisualElement schemaControls = new();

            PropertyField schemaField = SubGraphSerializer.DrawSchemaFieldWithCallback((prop) =>
            {
                if (schemaFoldout.visible && SubGraph.Schema == null)
                {
                    schemaFoldout.visible = false;
                    schemaControls.Clear();
                }
                else if (!schemaFoldout.visible && SubGraph.Schema != null)
                {
                    schemaControls.Add(SubGraphSerializer.SchemaSerializer.DrawEgressPortSelectorGUI());
                    schemaControls.Add(SubGraphSerializer.SchemaSerializer.DrawSchemaUpdaterButtonGUI());
                    schemaFoldout.visible = true;
                }
            }, visible: false);
            schemaControls.Add(SubGraphSerializer.SchemaSerializer?.DrawEgressPortSelectorGUI());
            schemaControls.Add(SubGraphSerializer.SchemaSerializer?.DrawSchemaUpdaterButtonGUI());

            schemaFoldout.Add(schemaField);
            schemaFoldout.Add(schemaControls);

            return schemaFoldout;
        }
    }
}