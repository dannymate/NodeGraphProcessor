using UnityEngine;
using System.Collections.Generic;
using System;

namespace GraphProcessor
{
    [Serializable]
    public class SubGraph : BaseGraph
    {
        public const string IngressPortDataFieldName = nameof(ingressPortData);
        public const string EgressPortDataFieldName = nameof(egressPortData);
        public const string SchemaFieldName = nameof(schema);
        public const string OptionsFieldName = nameof(options);
        public const string IsMacroFieldName = nameof(isMacro);
        public const string MacroOptionsFieldName = nameof(macroOptions);

        public event Notify OnPortsUpdated;
        public event Notify OnOptionsUpdated;

        [SerializeField]
        private SubGraphOptions options;

        [SerializeField]
        private bool isMacro = false;

        [SerializeField]
        private MacroOptions macroOptions;

        [SerializeField]
        private List<PortData> ingressPortData = new();

        [SerializeField]
        private List<PortData> egressPortData = new();

        [SerializeField]
        private SubGraphPortSchema schema;


        public bool IsMacro => isMacro;
        public SubGraphOptions Options => options;
        public MacroOptions MacroOptions => macroOptions;
        public SubGraphPortSchema Schema => schema;

        public List<PortData> IngressPortData
        {
            get
            {
                var portData = new List<PortData>();
                if (schema) portData.AddRange(schema.ingressPortData);
                portData.AddRange(ingressPortData);
                return portData;
            }
        }

        public List<PortData> EgressPortData
        {
            get
            {
                var portData = new List<PortData>();
                if (schema) portData.AddRange(schema.egressPortData);
                portData.AddRange(egressPortData);
                return portData;
            }
        }

        [NonSerialized]
        private SubGraphIngressNode _ingressNode;
        public SubGraphIngressNode IngressNode =>
                    PropertyUtils.LazyLoad(ref _ingressNode, () => GraphUtils.FindNodeInGraphOfType<SubGraphIngressNode>(this));


        [NonSerialized]
        private SubGraphEgressNode _egressNode;
        public SubGraphEgressNode EgressNode =>
            PropertyUtils.LazyLoad(ref _egressNode, () => GraphUtils.FindNodeInGraphOfType<SubGraphEgressNode>(this));

        protected override void OnEnable()
        {
            base.OnEnable();
            schema?.AddUpdatePortsListener(NotifyPortsChanged);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (IngressNode == null || !nodesPerGUID.ContainsKey(IngressNode.GUID))
            {
                if (IngressNode != null)
                    this.RemoveNode(IngressNode);

                _ingressNode = BaseNode.CreateFromType<SubGraphIngressNode>(Vector2.zero);
                AddNode(_ingressNode);
            }

            if (EgressNode == null || !nodesPerGUID.ContainsKey(EgressNode.GUID))
            {
                if (EgressNode != null)
                    this.RemoveNode(EgressNode);


                _egressNode = BaseNode.CreateFromType<SubGraphEgressNode>(Vector2.zero);
                AddNode(_egressNode);
            }
        }

        public void AddIngressPort(PortData portData)
        {
            ingressPortData.Add(portData);
        }

        public void AddReturnPort(PortData portData)
        {
            egressPortData.Add(portData);
        }

        public void AddUpdatePortsListener(Notify listener)
        {
            this.OnPortsUpdated += listener;
        }

        public void RemoveUpdatePortsListener(Notify listener)
        {
            this.OnPortsUpdated -= listener;
        }

        public void AddOptionsListener(Notify listener)
        {
            this.OnOptionsUpdated += listener;
        }

        public void RemoveOptionsListener(Notify listener)
        {
            this.OnOptionsUpdated -= listener;
        }

        public void NotifyPortsChanged()
        {
            OnPortsUpdated?.Invoke();
        }

        public void NotifyOptionsChanged()
        {
            OnOptionsUpdated?.Invoke();
        }
    }
}