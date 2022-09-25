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
        // Port creation here instead of SubGraphNode
        // Creation of a node that returns values externally (Return)
        // Creation of a node that brings in values externally (Start)
        // It may be useful to have multiple places to edit the ports, Start Return Graph SubGraphNode
        // Custom process function that takes inputs and returns the values
        // Possibly create GUI methods for nodes to use like in FlowCanvas
        // Split show inspector and node inspector because its annoying

        [SerializeField]
        public List<PortData> inputData;

        [SerializeField]
        public List<PortData> outputData;

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
                    _inputDataSerialized = ThisSerialized.FindProperty(nameof(inputData));
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
                    _outputDataSerialized = ThisSerialized.FindProperty(nameof(outputData));
                }
                return _outputDataSerialized;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (inputData.Count > 0 && IngressNode == null)//
            {
                var ingressNode = BaseNode.CreateFromType<IngressNode>(Vector2.zero);
                AddNode(ingressNode);
            }

            if (outputData.Count > 0 && (ReturnNode == null))
            {
                var returnNode = BaseNode.CreateFromType<ReturnNode>(Vector2.zero);
                AddNode(returnNode);
            }
        }

        public void AddIngressPort(PortData portData)
        {
            inputData.Add(portData);
        }

        public void AddReturnPort(PortData portData)
        {
            outputData.Add(portData);
        }

        public override void CreateInspectorGUI(VisualElement root)
        {
            base.CreateInspectorGUI(root);
            DrawPortSelectionGUI(root);
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
            VisualElement inputData = new PropertyField(InputDataSerialized);
            VisualElement outputData = new PropertyField(OutputDataSerialized);
            VisualElement updatePortsButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE PORTS" };

            inputData.Bind(ThisSerialized);
            outputData.Bind(ThisSerialized);

            root.Add(inputData);
            root.Add(outputData);
            root.Add(updatePortsButton);
        }

        public void NotifyPortsChanged()
        {
            PortsUpdated?.Invoke();
        }

        public event Notify PortsUpdated; // event
        public delegate void Notify();  // delegate
    }
}