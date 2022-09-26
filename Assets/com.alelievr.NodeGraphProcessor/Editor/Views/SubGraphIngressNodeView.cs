using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(SubGraphIngressNode))]
    public class SubgraphIngressNodeView : BaseNodeView
    {
        SubGraphIngressNode Target => this.nodeTarget as SubGraphIngressNode;
        SubGraph SubGraph => Target.SubGraph;

        public override void Enable()
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

            subgraphPortFoldout.Add(SubGraph.DrawInputDataGUI());
            subgraphPortFoldout.Add(SubGraph.DrawUpdateSchemaButtonGUI());

            return subgraphPortFoldout;

        }

        public VisualElement DrawSchemaControlsGUI()
        {
            var schemaFoldout = new Foldout()
            {
                text = "Schema Port Selection"
            };
            VisualElement schemaControls = new();

            PropertyField schemaField = SubGraph.DrawSchemaFieldGUI();
            schemaField.visible = false;
            schemaField.style.height = 0;
            schemaField.RegisterValueChangeCallback((prop) =>
            {
                if (schemaFoldout.visible && SubGraph.Schema == null)
                {
                    schemaFoldout.visible = false;
                    schemaControls.Clear();
                }
                else if (!schemaFoldout.visible && SubGraph.Schema != null)
                {
                    schemaControls.Add(SubGraph.Schema.DrawInputDataGUI());
                    schemaControls.Add(SubGraph.Schema.DrawUpdateSchemaButtonGUI());
                    schemaFoldout.visible = true;
                }
            });
            schemaControls.Add(SubGraph.Schema?.DrawInputDataGUI());
            schemaControls.Add(SubGraph.Schema?.DrawUpdateSchemaButtonGUI());

            schemaFoldout.Add(schemaField);
            schemaFoldout.Add(schemaControls);

            return schemaFoldout;
        }
    }
}