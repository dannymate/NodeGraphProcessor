using System.Collections.Generic;

namespace GraphProcessor
{
    public class NodeDelegates
    {
        public delegate IEnumerable<PortData> CustomPortBehaviorDelegate(List<SerializableEdge> edges);
        public delegate IEnumerable<PortData> CustomPortTypeBehaviorDelegate(string fieldName, string displayName, object value);

        public class CustomPortBehaviorDelegateInfo
        {
            private readonly CustomPortBehaviorDelegate _deleg;
            private readonly bool _cloneResults;

            public CustomPortBehaviorDelegateInfo(CustomPortBehaviorDelegate deleg, bool cloneResults)
            {
                this._deleg = deleg;
                this._cloneResults = cloneResults;
            }

            public CustomPortBehaviorDelegate Delegate => _deleg;
            public bool CloneResults => _cloneResults;

        }

        public class CustomPortTypeBehaviorDelegateInfo
        {
            private readonly CustomPortTypeBehaviorDelegate _deleg;
            private readonly bool _cloneResults;

            public CustomPortTypeBehaviorDelegateInfo(CustomPortTypeBehaviorDelegate deleg, bool cloneResults)
            {
                this._deleg = deleg;
                this._cloneResults = cloneResults;
            }

            public CustomPortTypeBehaviorDelegate Delegate => _deleg;
            public bool CloneResults => _cloneResults;
        }
    }
}