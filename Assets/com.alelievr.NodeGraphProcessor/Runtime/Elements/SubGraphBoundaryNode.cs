using System.Collections.Generic;

namespace GraphProcessor
{
    [System.Serializable]
    public abstract class SubGraphBoundaryNode : BaseNode
    {
        protected Dictionary<PortData, object> passThroughBufferByPort = new();


        protected abstract List<PortData> Ports { get; }
        public SubGraph SubGraph => graph as SubGraph;

        public override bool isRenamable => false;
        public override bool deletable => false;
        public override bool needsInspector => true;
        public override bool HideNodeInspectorBlock => true;

        public sealed override void InitializePorts()
        {
            base.InitializePorts();
            SubGraph.AddUpdatePortsListener(OnPortsListUpdated);
            passThroughBufferByPort.Clear();
        }

        protected virtual void OnPortsListUpdated() => UpdateAllPortsLocal();
    }
}