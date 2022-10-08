using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(SubGraphIngressNode))]
    public class SubgraphIngressNodeView : BaseNodeView
    {
        SubGraphIngressNode Target => this.nodeTarget as SubGraphIngressNode;
        SubGraph SubGraph => Target.SubGraph;

        SubGraphGUIUtility _subGraphSerializer;
        SubGraphGUIUtility SubGraphSerializer =>
            PropertyUtils.LazyLoad(ref _subGraphSerializer, () => new(SubGraph));

        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            controlsContainer.Add(DrawSubGraphControlsGUI());
            controlsContainer.Add(DrawSchemaControlsGUI());
        }

        public VisualElement DrawSubGraphControlsGUI()
        {
            var subgraphPortFoldout = new Foldout()
            {
                text = "SubGraph Port Selection"
            };

            subgraphPortFoldout.Add(SubGraphSerializer.DrawIngressPortSelectorGUI());
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
                    schemaControls.Add(SubGraphSerializer.SchemaGUIUtil.DrawIngressPortSelectorGUI());
                    schemaControls.Add(SubGraphSerializer.SchemaGUIUtil.DrawSchemaUpdaterButtonGUI());
                    schemaFoldout.visible = true;
                }
            }, visible: false);

            schemaControls.Add(SubGraphSerializer.SchemaGUIUtil?.DrawIngressPortSelectorGUI());
            schemaControls.Add(SubGraphSerializer.SchemaGUIUtil?.DrawSchemaUpdaterButtonGUI());

            schemaFoldout.Add(schemaField);
            schemaFoldout.Add(schemaControls);

            return schemaFoldout;
        }
    }
}