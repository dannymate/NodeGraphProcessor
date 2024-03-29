﻿using System;
using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    public class SerializableEdge : ISerializationCallbackReceiver
    {
        [SerializeField]
        private PropertyName _guid;
        public PropertyName GUID
        {
            get
            {
                if (PropertyName.IsNullOrEmpty(_guid))
                    _guid = Guid.NewGuid().ToString();

                return _guid;
            }
        }

        [SerializeField]
        BaseGraph owner;

        [SerializeField]
        PropertyName inputNodeGUID;
        [SerializeField]
        PropertyName outputNodeGUID;

        [System.NonSerialized]
        public BaseNode inputNode;

        [System.NonSerialized]
        public NodePort inputPort;
        [System.NonSerialized]
        public NodePort outputPort;

        //temporary object used to send port to port data when a custom input/output function is used.
        [System.NonSerialized]
        public object passThroughBuffer;

        [System.NonSerialized]
        public BaseNode outputNode;

        public string inputFieldName;
        public string outputFieldName;

        // Use to store the id of the field that generate multiple ports
        public string inputPortIdentifier;
        public string outputPortIdentifier;

        public SerializableEdge() { }

        public static SerializableEdge CreateNewEdge(BaseGraph graph, NodePort inputPort, NodePort outputPort)
        {
            SerializableEdge edge = new SerializableEdge();

            edge.owner = graph;
            edge._guid = Guid.NewGuid().ToString();
            edge.inputNode = inputPort.owner;
            edge.inputFieldName = inputPort.fieldName;
            edge.outputNode = outputPort.owner;
            edge.outputFieldName = outputPort.fieldName;
            edge.inputPort = inputPort;
            edge.outputPort = outputPort;
            edge.inputPortIdentifier = inputPort.portData.Identifier;
            edge.outputPortIdentifier = outputPort.portData.Identifier;

            return edge;
        }

        public void OnBeforeSerialize()
        {
            if (outputNode == null || inputNode == null)
                return;

            outputNodeGUID = outputNode.GUID;
            inputNodeGUID = inputNode.GUID;
        }

        public void OnAfterDeserialize() { }

        //here our owner have been deserialized
        public void Deserialize()
        {
            if (!owner.nodesPerGUID.ContainsKey(outputNodeGUID) || !owner.nodesPerGUID.ContainsKey(inputNodeGUID))
                return;

            outputNode = owner.nodesPerGUID[outputNodeGUID];
            inputNode = owner.nodesPerGUID[inputNodeGUID];
            inputPort = inputNode.GetPort(inputFieldName, inputPortIdentifier);
            outputPort = outputNode.GetPort(outputFieldName, outputPortIdentifier);
        }

        public override string ToString() => $"{outputNode.name}:{outputPort.fieldName} -> {inputNode.name}:{inputPort.fieldName}";
    }
}
