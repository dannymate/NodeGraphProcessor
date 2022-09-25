using UnityEngine;
using System.Collections.Generic;
using TypeReferences;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using GraphProcessor;

namespace GraphProcessor
{
    [Serializable]
    public class SubGraph : BaseGraph
    {
        // Possibly create GUI methods for nodes to use like in FlowCanvas
        public event Notify OnPortsUpdated;


        [SerializeField]
        private List<PortData> localInputData;

        [SerializeField]
        private List<PortData> localOutputData;

        [SerializeField]
        private SubGraphPortSchema schema;

        public SubGraphPortSchema Schema => schema;

        public List<PortData> InputData
        {
            get
            {
                var portData = new List<PortData>(schema.inputData);
                portData.AddRange(localInputData);
                return portData;
            }
        }

        public List<PortData> OutputData
        {
            get
            {
                var portData = new List<PortData>(schema.outputData);
                portData.AddRange(localOutputData);
                return portData;
            }
        }


        [SerializeField, HideInInspector]
        public IngressNode IngressNode => nodes.Find(x => x.GetType() == typeof(IngressNode)) as IngressNode;

        [SerializeField, HideInInspector]
        public ReturnNode ReturnNode => nodes.Find(x => x.GetType() == typeof(ReturnNode)) as ReturnNode;

        SerializedObject _thisSerialized;
        public SerializedObject ThisSerialized
        {
            get
            {
                if (_thisSerialized == null)
                {
                    _thisSerialized = new SerializedObject(this);
                }
                return _thisSerialized;
            }
        }

        SerializedProperty _inputDataSerialized;
        public SerializedProperty InputDataSerialized
        {
            get
            {
                if (_inputDataSerialized == null)
                {
                    _inputDataSerialized = ThisSerialized.FindProperty(nameof(localInputData));
                }
                return _inputDataSerialized;
            }
        }

        SerializedProperty _outputDataSerialized;
        public SerializedProperty OutputDataSerialized
        {
            get
            {
                if (_outputDataSerialized == null)
                {
                    _outputDataSerialized = ThisSerialized.FindProperty(nameof(localOutputData));
                }
                return _outputDataSerialized;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            schema?.AddUpdatePortsListener(NotifyPortsChanged);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (localInputData.Count > 0 && IngressNode == null)//
            {
                var ingressNode = BaseNode.CreateFromType<IngressNode>(Vector2.zero);
                AddNode(ingressNode);
            }

            if (localOutputData.Count > 0 && (ReturnNode == null))
            {
                var returnNode = BaseNode.CreateFromType<ReturnNode>(Vector2.zero);
                AddNode(returnNode);
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

            DrawPortSelectionGUI(root);
            DrawSchemaControlGUI(root);
        }

        private void DrawSchemaControlGUI(VisualElement root)
        {
            VisualElement schemaControlsFoldout = new Foldout()
            {
                text = "Schema Controls"
            };

            DrawSchemaFieldGUI(schemaControlsFoldout);
            schema?.DrawControlGUI(schemaControlsFoldout);

            root.Add(schemaControlsFoldout);
        }

        public void DrawSchemaFieldGUI(VisualElement root)
        {
            VisualElement schemaField = new PropertyField(ThisSerialized.FindProperty(nameof(schema)));
            schemaField.Bind(ThisSerialized);
            root.Add(schemaField);
        }

        public void DrawPortSelectionGUI(VisualElement root)
        {
            VisualElement portSelectionFoldout = new Foldout()
            {
                text = "Port Selection"
            };

            DrawInputDataGUI(portSelectionFoldout);
            DrawOutputDataGUI(portSelectionFoldout);
            DrawUpdateSchemaButtonGUI(portSelectionFoldout);

            root.Add(portSelectionFoldout);
        }

        public void DrawInputDataGUI(VisualElement root)
        {
            VisualElement inputData = new PropertyField(InputDataSerialized);
            inputData.Bind(ThisSerialized);
            root.Add(inputData);
        }

        public void DrawOutputDataGUI(VisualElement root)
        {
            VisualElement outputData = new PropertyField(OutputDataSerialized);
            outputData.Bind(ThisSerialized);
            root.Add(outputData);
        }

        public void DrawUpdateSchemaButtonGUI(VisualElement root)
        {
            VisualElement updatePortsButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE PORTS" };
            root.Add(updatePortsButton);
        }

        public void NotifyPortsChanged()
        {
            OnPortsUpdated?.Invoke();
        }
    }
}