using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using GraphProcessor.Utils;

namespace GraphProcessor
{
    public class SubGraphGUIUtility
    {
        readonly SubGraph _subgraph;
        SerializedObject _subGraphSerialized;
        SerializedProperty _ingressPortDataSerialized;
        SerializedProperty _egressPortDataSerialized;
        SerializedProperty _schemaSerialized;
        SerializedProperty _isMacro;
        SerializedProperty _macroOptions;

        public SubGraphGUIUtility(SubGraph subGraph)
        {
            this._subgraph = subGraph;
        }

        public SubGraph SubGraph => _subgraph;
        public SubGraphOptionsGUIUtility OptionsGUIUtil => new(SubGraph);
        public SubGraphSchemaGUIUtility SchemaGUIUtil => SubGraph.Schema ? new(SubGraph.Schema) : null;

        public SerializedObject SubGraphObject =>
            PropertyUtils.LazyLoad(
                ref _subGraphSerialized,
                () => new SerializedObject(SubGraph)
            );

        public SerializedProperty IngressPortData =>
            PropertyUtils.LazyLoad(
                ref _ingressPortDataSerialized,
                () => SubGraphObject.FindProperty(SubGraph.IngressPortDataFieldName)
            );

        public SerializedProperty EgressPortData =>
            PropertyUtils.LazyLoad(
                ref _egressPortDataSerialized,
                () => SubGraphObject.FindProperty(SubGraph.EgressPortDataFieldName)
            );


        public SerializedProperty Schema =>
            PropertyUtils.LazyLoad(
                ref _schemaSerialized,
                () => SubGraphObject.FindProperty(SubGraph.SchemaFieldName)
            );

        public SerializedProperty IsMacro =>
            PropertyUtils.LazyLoad(
                ref _isMacro,
                () => SubGraphObject.FindProperty(SubGraph.IsMacroFieldName)
            );

        public SerializedProperty MacroOptions =>
            PropertyUtils.LazyLoad(
                ref _macroOptions,
                () => SubGraphObject.FindProperty(SubGraph.MacroOptionsFieldName)
            );

        public VisualElement DrawSubGraphPortControlGUI()
        {
            var portSelectionFoldout = new Foldout()
            {
                text = "SubGraph Port Control"
            };

            portSelectionFoldout.Add(DrawIngressPortSelectorGUI(bind: false));
            portSelectionFoldout.Add(DrawEgressPortSelectorGUI(bind: false));
            portSelectionFoldout.Add(DrawPortUpdaterButtonGUI());

            portSelectionFoldout.Bind(SubGraphObject);

            return portSelectionFoldout;
        }

        public PropertyField DrawSchemaFieldGUI(bool bind = true)
        {
            var schemaField = new PropertyField(Schema);
            if (bind) schemaField.Bind(SubGraphObject);
            return schemaField;
        }

        public PropertyField DrawSchemaFieldWithCallback(EventCallback<SerializedPropertyChangeEvent> onChangeCallback, bool visible = true, bool bind = true)
        {
            PropertyField schemaField = DrawSchemaFieldGUI(bind);
            schemaField.RegisterValueChangeCallback(onChangeCallback);
            if (!visible)
            {
                schemaField.visible = false;
                schemaField.style.height = 0;
            }
            return schemaField;
        }

        public PropertyField DrawIngressPortSelectorGUI(bool bind = true)
        {
            var ingressDataField = new PropertyField(IngressPortData) { label = "Ingress Port Data - SubGraph" };
            if (bind) ingressDataField.Bind(SubGraphObject);
            return ingressDataField;
        }

        public PropertyField DrawEgressPortSelectorGUI(bool bind = true)
        {
            var egressDataField = new PropertyField(EgressPortData) { label = "Egress Port Data - SubGraph" };
            if (bind) egressDataField.Bind(SubGraphObject);
            return egressDataField;
        }

        public Button DrawPortUpdaterButtonGUI()
        {
            var updateSchemaButton = new Button(() => SubGraph.NotifyPortsChanged()) { text = "UPDATE PORTS" };
            return updateSchemaButton;
        }

        public VisualElement DrawOptionsGUI()
        {
            return OptionsGUIUtil.DrawGUI();
        }

        public VisualElement DrawMacroOptionsGUI()
        {
            VisualElement root = new();
            VisualElement macroOptionsContainer = new();

            var isMacroField = new PropertyField(IsMacro);
            isMacroField.RegisterValueChangeCallback((prop) =>
            {
                if (macroOptionsContainer.IsShowing() && !SubGraph.IsMacro)
                {
                    macroOptionsContainer.Hide();
                }
                else if (!macroOptionsContainer.IsShowing() && SubGraph.IsMacro)
                {
                    macroOptionsContainer.Show();
                }
            });

            var macroOptionsField = new PropertyField(MacroOptions);
            macroOptionsField.RegisterCallback<ChangeEvent<MacroOptions>>((prop) =>
            {
                Debug.Log("Changed");
            });
            isMacroField.Bind(SubGraphObject);
            macroOptionsField.Bind(SubGraphObject);

            macroOptionsContainer.Add(macroOptionsField);

            root.Add(isMacroField);
            root.Add(macroOptionsContainer);

            return root;
        }
    }
}