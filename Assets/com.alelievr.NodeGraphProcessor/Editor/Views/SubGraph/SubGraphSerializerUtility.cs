using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class SubGraphSerializerUtility
    {
        readonly SubGraph _subgraph;
        SerializedObject _subGraphSerialized;
        SerializedProperty _ingressPortDataSerialized;
        SerializedProperty _egressPortDataSerialized;
        SerializedProperty _schemaSerialized;
        SerializedProperty _isMacro;
        SerializedProperty _menuLocation;

        public SubGraphSerializerUtility(SubGraph subGraph)
        {
            this._subgraph = subGraph;
        }

        public SubGraph SubGraph => _subgraph;
        public SubGraphSchemaSerializer SchemaSerializer => SubGraph.Schema ? new(SubGraph.Schema) : null;

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

        public SerializedProperty MenuLocation =>
            PropertyUtils.LazyLoad(
                ref _menuLocation,
                () => SubGraphObject.FindProperty(SubGraph.MenuLocationFieldName)
            );

        public VisualElement DrawFullSubGraphGUI()
        {
            var portSelectionFoldout = new Foldout()
            {
                text = "SubGraph Port Selection"
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
            var ingressDataField = new PropertyField(IngressPortData);
            if (bind) ingressDataField.Bind(SubGraphObject);
            return ingressDataField;
        }

        public PropertyField DrawEgressPortSelectorGUI(bool bind = true)
        {
            var egressDataField = new PropertyField(EgressPortData);
            if (bind) egressDataField.Bind(SubGraphObject);
            return egressDataField;
        }

        public Button DrawPortUpdaterButtonGUI()
        {
            var updateSchemaButton = new Button(() => SubGraph.NotifyPortsChanged()) { text = "UPDATE PORTS" };
            return updateSchemaButton;
        }

        public Foldout DrawMacroGUI()
        {
            var macroFoldout = new Foldout()
            {
                text = "Macro Options"
            };

            VisualElement macroOptions = new();

            var isMacroField = new PropertyField(IsMacro);
            isMacroField.RegisterValueChangeCallback((prop) =>
            {
                if (macroOptions.visible == true && !SubGraph.IsMacro)
                {
                    macroOptions.visible = false;
                }
                else if (macroOptions.visible == false && SubGraph.IsMacro)
                {
                    macroOptions.visible = true;
                }
            });

            var menuLocationField = new PropertyField(MenuLocation);

            isMacroField.Bind(SubGraphObject);
            menuLocationField.Bind(SubGraphObject);

            macroOptions.Add(menuLocationField);

            macroFoldout.Add(isMacroField);
            macroFoldout.Add(macroOptions);

            return macroFoldout;
        }
    }
}