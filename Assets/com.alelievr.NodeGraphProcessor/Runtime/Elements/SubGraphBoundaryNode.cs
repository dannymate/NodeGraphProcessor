using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    [NodeOpacityIfNoPorts(0.25f)]
    public abstract class SubGraphBoundaryNode : BaseNode
    {
        protected Dictionary<PortData, object> passThroughBufferByPort = new();

        protected override NodeRenamePolicy DefaultRenamePolicy => NodeRenamePolicy.DISABLED;
        public override bool deletable => false;
        public override bool needsInspector => true;
        public override bool HideNodeInspectorBlock => true;
        public override Color AccentColor => Color.grey;

        public SubGraph SubGraph => graph as SubGraph;

        protected abstract List<PortData> Ports { get; }

        public sealed override void InitializePorts()
        {
            base.InitializePorts();
            SubGraph.AddUpdatePortsListener(OnPortsListUpdated);
            passThroughBufferByPort.Clear();
        }

        protected virtual void OnPortsListUpdated() => UpdateAllPortsLocal();
    }
}