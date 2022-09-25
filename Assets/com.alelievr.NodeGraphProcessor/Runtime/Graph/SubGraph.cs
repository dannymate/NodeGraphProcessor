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

        [SerializeField]
        private List<PortData> localInputData;

        [SerializeField]
        private List<PortData> localOutputData;

        [SerializeField]
        private SubGraphPortSchema schema;

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

        public override void CreateInspectorGUI(VisualElement root)
        {
            base.CreateInspectorGUI(root);
            DrawPortSelectionGUI(root);
            VisualElement schemaField = new PropertyField(ThisSerialized.FindProperty(nameof(schema)));
            schemaField.Bind(ThisSerialized);
            root.Add(schemaField);
        }

        public void AddUpdatePortsListener(Notify listener)
        {
            this.PortsUpdated += listener;
        }

        public void RemoveUpdatePortsListener(Notify listener)
        {
            this.PortsUpdated -= listener;
        }

        public void DrawPortSelectionGUI(VisualElement root)
        {
            VisualElement portSelectionFoldout = new Foldout()
            {
                text = "Port Selection"
            };
            VisualElement inputData = new PropertyField(InputDataSerialized);
            VisualElement outputData = new PropertyField(OutputDataSerialized);
            VisualElement updatePortsButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE PORTS" };

            inputData.Bind(ThisSerialized);
            outputData.Bind(ThisSerialized);

            portSelectionFoldout.Add(inputData);
            portSelectionFoldout.Add(outputData);
            portSelectionFoldout.Add(updatePortsButton);

            root.Add(portSelectionFoldout);
        }

        public void NotifyPortsChanged()
        {
            PortsUpdated?.Invoke();
        }

        public event Notify PortsUpdated; // event
    }
}