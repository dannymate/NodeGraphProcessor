using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using GraphProcessor.Utils;

namespace GraphProcessor.View
{
    [NodeCustomEditor(typeof(SubGraphNode))]
    public class SubGraphNodeView : BaseNodeView
    {
        SubGraphNode Target => nodeTarget as SubGraphNode;
        SubGraph SubGraph => Target.SubGraph;

        SubGraphGUIUtility SubGraphSerializer => SubGraph ? new(SubGraph) : null;

        public override void OnDoubleClicked()
        {
            if (SubGraph == null) return;

            EditorWindow.GetWindow<SubGraphWindow>().InitializeGraph(SubGraph);
        }

        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            VisualElement subGraphGUIContainer = new();
            if (SubGraph != null)
                subGraphGUIContainer.Add(DrawSubGraphGUI());


            controlsContainer.Add(subGraphGUIContainer);
            controlsContainer.Add(DrawSubGraphField(subGraphGUIContainer));
        }

        protected VisualElement DrawSubGraphGUI()
        {
            VisualElement subGraphGUIContainer = new();

            subGraphGUIContainer.Add(SubGraphSerializer?.DrawFullSubGraphGUI());
            subGraphGUIContainer.Add(DrawSchemaControls());

            return subGraphGUIContainer;
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
                    schemaControls.Add(SubGraphSerializer.SchemaGUIUtil.DrawFullSchemaGUI());
                    schemaControls.visible = true;
                }
            }, visible: false);
            schemaControls.Add(schemaField);
            schemaControls.Add(SubGraphSerializer.SchemaGUIUtil?.DrawFullSchemaGUI());

            return schemaControls;
        }

        protected VisualElement DrawSubGraphField(VisualElement subGraphGUIContainer)
        {
            ObjectField subGraphField = new("SubGraph")
            {
                objectType = typeof(SubGraph),
                value = Target.SubGraph
            };
            subGraphField.RegisterValueChangedCallback((prop) =>
            {
                Target.SetPrivateFieldValue(SubGraphNode.SubGraphField, prop.newValue as SubGraph);
                Target.UpdateAllPortsLocal();
                Target.RepaintTitle();

                if (prop.newValue == null)
                {
                    subGraphGUIContainer.Hide();
                    subGraphGUIContainer.Clear();
                }
                else
                {
                    subGraphGUIContainer.Add(DrawSubGraphGUI());
                    subGraphGUIContainer.Show();
                }
            });
            return subGraphField;
        }
    }
}