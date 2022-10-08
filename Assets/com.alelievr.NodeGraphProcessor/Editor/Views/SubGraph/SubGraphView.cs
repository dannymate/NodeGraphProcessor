using GraphProcessor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CustomEditor(typeof(SubGraph), true)]
    public class SubGraphView : GraphInspector
    {
        protected SubGraph SubGraph => target as SubGraph;
        protected SubGraphPortSchema Schema => SubGraph.Schema;

        SubGraphGUIUtility _subGraphSerializer;
        SubGraphGUIUtility SubGraphSerializer =>
            PropertyUtils.LazyLoad(ref _subGraphSerializer, () => new(SubGraph));

        protected override void CreateInspector()
        {
            base.CreateInspector();

            var optionsGUI = SubGraphSerializer.DrawOptionsGUI();
            optionsGUI.Add(SubGraphSerializer.DrawMacroOptionsGUI());
            root.Add(optionsGUI);
            root.Add(SubGraphSerializer.DrawSubGraphPortControlGUI());
            root.Add(DrawSchemaControlGUI());
        }

        private VisualElement DrawSchemaControlGUI()
        {
            VisualElement schemaControlsFoldout = new Foldout()
            {
                text = "Schema Controls"
            };

            VisualElement schemaControls = new();
            schemaControls.Add(SubGraphSerializer.SchemaGUIUtil?.DrawSchemaPortControlGUI());

            PropertyField schemaField = SubGraphSerializer.DrawSchemaFieldGUI();
            schemaField.RegisterCallback<ChangeEvent<Object>>((e) =>
            {
                SubGraphPortSchema prevSchemaValue = e.previousValue as SubGraphPortSchema;
                SubGraphPortSchema newSchemaValue = e.newValue as SubGraphPortSchema;

                if (prevSchemaValue == newSchemaValue) return;
                if (prevSchemaValue) prevSchemaValue.OnPortsUpdated -= SubGraph.NotifyPortsChanged;

                schemaControls.Clear();

                if (!newSchemaValue)
                {
                    schemaControls.Hide();
                }
                else
                {
                    newSchemaValue.OnPortsUpdated += SubGraph.NotifyPortsChanged;
                    schemaControls.Add(SubGraphSerializer.SchemaGUIUtil.DrawSchemaPortControlGUI());
                    schemaControls.Show();
                }
            });

            schemaControlsFoldout.Add(schemaField);
            schemaControlsFoldout.Add(schemaControls);
            schemaControlsFoldout.Add(SubGraphSchemaGUIUtility.DrawSchemaUpdaterButtonGUI(() =>
            {
                if (SubGraph.Schema == null) SubGraph.NotifyPortsChanged();
                else SubGraphSerializer.SchemaGUIUtil.SchemaUpdateButtonAction();
            }));

            return schemaControlsFoldout;
        }
    }
}