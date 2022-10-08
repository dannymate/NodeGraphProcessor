#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public partial class BaseGraph
    {
        public virtual void CreateInspectorGUI(VisualElement root) { }
    }
}

#endif
