using UnityEditor;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CustomEditor(typeof(SubGraphPortSchema))]
    public class SubGraphPortSchema_Inspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var schema = target as SubGraphPortSchema;

            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new();

            // Add a simple label
            root.Add(schema.DrawControlGUI());

            // Return the finished inspector UI
            return root;
        }
    }
}