using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CustomEditor(typeof(SubGraph), true)]
    public partial class SubGraphView : GraphInspector
    {
        protected SubGraph SubGraph => target as SubGraph;
        protected SubGraphPortSchema Schema => SubGraph.Schema;

        SubGraphSerializerUtility _subGraphSerializer;
        SubGraphSerializerUtility SubGraphSerializer =>
            PropertyUtils.LazyLoad(ref _subGraphSerializer, () => new(SubGraph));

        protected override void CreateInspector()
        {
            base.CreateInspector();

            root.Add(SubGraphSerializer.DrawFullSubGraphGUI());
            root.Add(DrawSchemaControlGUI());
            root.Add(SubGraphSerializer.DrawMacroGUI());
        }

        private VisualElement DrawSchemaControlGUI()
        {
            VisualElement schemaControlsFoldout = new Foldout()
            {
                text = "Schema Controls"
            };

            VisualElement schemaControls = new();
            schemaControls.Add(SubGraphSerializer.SchemaSerializer?.DrawFullSchemaGUI());

            PropertyField schemaField = SubGraphSerializer.DrawSchemaFieldGUI();
            schemaField.RegisterValueChangeCallback((prop) =>
            {
                // We sanity check visibility due to this callback being called twice
                if (schemaControls.visible && Schema == null)
                {
                    schemaControls.visible = false;
                    schemaControls.Clear();
                }
                else if (!schemaControls.visible && Schema != null)
                {
                    schemaControls.Add(SubGraphSerializer.SchemaSerializer.DrawFullSchemaGUI());
                    schemaControls.visible = true;
                }
            });

            schemaControlsFoldout.Add(schemaField);
            schemaControlsFoldout.Add(schemaControls);

            return schemaControlsFoldout;
        }
    }
}