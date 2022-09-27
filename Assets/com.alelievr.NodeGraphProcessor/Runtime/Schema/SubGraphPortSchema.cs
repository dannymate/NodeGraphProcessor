using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CreateAssetMenu(fileName = "SubGraphPortSchema", menuName = "NGP/Schema/SubGraphPortSchema", order = 0)]
    public class SubGraphPortSchema : ScriptableObject
    {
        public const string IngressPortDataFieldName = nameof(ingressPortData);
        public const string EgressPortDataFieldName = nameof(egressPortData);

        public event Notify OnPortsUpdated;

        [SerializeField]
        public List<PortData> ingressPortData;

        [SerializeField]
        public List<PortData> egressPortData;

        public void NotifyPortsChanged()
        {
            OnPortsUpdated?.Invoke();
        }

        public void AddUpdatePortsListener(Notify listener)
        {
            OnPortsUpdated += listener;
        }

        public void RemoveUpdatePortsListener(Notify listener)
        {
            OnPortsUpdated -= listener;
        }
    }
}