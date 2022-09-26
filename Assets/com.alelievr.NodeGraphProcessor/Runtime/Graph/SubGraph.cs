using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace GraphProcessor
{
    [Serializable]
    public class SubGraph : BaseGraph
    {
        // Possibly create GUI methods for nodes to use like in FlowCanvas
        public event Notify OnPortsUpdated;


        [SerializeField]
        private List<PortData> localInputData = new();

        [SerializeField]
        private List<PortData> localOutputData = new();

        [SerializeField]
        private SubGraphPortSchema schema;
        public SubGraphPortSchema Schema => schema;

        public List<PortData> InputData
        {
            get
            {
                var portData = new List<PortData>();
                if (schema) portData.AddRange(schema.inputData);
                portData.AddRange(localInputData);
                return portData;
            }
        }

        public List<PortData> OutputData
        {
            get
            {
                var portData = new List<PortData>();
                if (schema) portData.AddRange(schema.outputData);
                portData.AddRange(localOutputData);
                return portData;
            }
        }

        private SubGraphIngressNode _ingressNode;
        public SubGraphIngressNode IngressNode =>
            PropertyUtils.LazyLoad(ref _ingressNode, () => GraphUtils.FindNodeInGraphOfType<SubGraphIngressNode>(this));

        private SubGraphEgressNode _egressNode;
        public SubGraphEgressNode EgressNode =>
            PropertyUtils.LazyLoad(ref _egressNode, () => GraphUtils.FindNodeInGraphOfType<SubGraphEgressNode>(this));

        SerializedObject _thisSerialized;
        public SerializedObject ThisSerialized =>
            PropertyUtils.LazyLoad(ref _thisSerialized, () => new SerializedObject(this));

        SerializedProperty _inputDataSerialized;
        public SerializedProperty InputDataSerialized =>
            PropertyUtils.LazyLoad(ref _inputDataSerialized, () => ThisSerialized.FindProperty(nameof(localInputData)));

        SerializedProperty _outputDataSerialized;
        public SerializedProperty OutputDataSerialized =>
            PropertyUtils.LazyLoad(ref _outputDataSerialized, () => ThisSerialized.FindProperty(nameof(localOutputData)));

        protected override void OnEnable()
        {
            base.OnEnable();
            schema?.AddUpdatePortsListener(NotifyPortsChanged);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (IngressNode == null)
            {
                _ingressNode = BaseNode.CreateFromType<SubGraphIngressNode>(Vector2.zero);
                AddNode(_ingressNode);
            }

            if (EgressNode == null)
            {
                _egressNode = BaseNode.CreateFromType<SubGraphEgressNode>(Vector2.zero);
                AddNode(_egressNode);
            }
        }

        public void AddIngressPort(PortData portData)
        {
            localInputData.Add(portData);
        }

        public void AddReturnPort(PortData portData)
        {
            localOutputData.Add(portData);
        }

        public void AddUpdatePortsListener(Notify listener)
        {
            this.OnPortsUpdated += listener;
        }

        public void RemoveUpdatePortsListener(Notify listener)
        {
            this.OnPortsUpdated -= listener;
        }

        public override void CreateInspectorGUI(VisualElement root)
        {
            base.CreateInspectorGUI(root);

            root.Add(DrawPortSelectionGUI());
            root.Add(DrawSchemaControlGUI());
        }

        private VisualElement DrawSchemaControlGUI()
        {
            VisualElement schemaControlsFoldout = new Foldout()
            {
                text = "Schema Controls"
            };

            VisualElement schemaControls = new();
            schemaControls.Add(schema?.DrawControlGUI());

            PropertyField schemaField = DrawSchemaFieldGUI();
            schemaField.RegisterValueChangeCallback((prop) =>
            {
                // We sanity check visibility due to this callback being called twice
                if (schemaControls.visible && schema == null)
                {
                    schemaControls.visible = false;
                    schemaControls.Clear();
                }
                else if (!schemaControls.visible && schema != null)
                {
                    schemaControls.Add(schema.DrawControlGUI());
                    schemaControls.visible = true;
                }
            });

            schemaControlsFoldout.Add(schemaControls);
            schemaControlsFoldout.Add(schemaField);

            return schemaControlsFoldout;
        }

        public PropertyField DrawSchemaFieldGUI()
        {
            var schemaField = new PropertyField(ThisSerialized.FindProperty(nameof(schema)));
            schemaField.Bind(ThisSerialized);
            return schemaField;
        }

        public VisualElement DrawPortSelectionGUI()
        {
            VisualElement portSelectionFoldout = new Foldout()
            {
                text = "Port Selection"
            };

            portSelectionFoldout.Add(DrawInputDataGUI(bind: false));
            portSelectionFoldout.Add(DrawOutputDataGUI(bind: false));
            portSelectionFoldout.Add(DrawUpdateSchemaButtonGUI());

            portSelectionFoldout.Bind(ThisSerialized);

            return portSelectionFoldout;
        }

        public PropertyField DrawInputDataGUI(bool bind = true)
        {
            var inputDataField = new PropertyField(InputDataSerialized);
            if (bind) inputDataField.Bind(ThisSerialized);
            return inputDataField;
        }

        public PropertyField DrawOutputDataGUI(bool bind = true)
        {
            var outputDataField = new PropertyField(OutputDataSerialized);
            if (bind) outputDataField.Bind(ThisSerialized);
            return outputDataField;
        }

        public Button DrawUpdateSchemaButtonGUI()
        {
            var updatePortsButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE PORTS" };
            return updatePortsButton;
        }

        public void NotifyPortsChanged()
        {
            OnPortsUpdated?.Invoke();
        }
    }
}