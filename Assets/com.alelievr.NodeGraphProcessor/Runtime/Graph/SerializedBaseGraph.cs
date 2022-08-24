using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace GraphProcessor
{
    [ShowOdinSerializedPropertiesInInspector]
    public class SerializedBaseGraph : GraphBase, ISerializationCallbackReceiver
    {
        [OdinSerialize]
        private List<BaseNode> nodes = new();
        public override List<BaseNode> Nodes => nodes;

        [SerializeField, HideInInspector]
        private SerializationData serializationData;


        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }
    }
}