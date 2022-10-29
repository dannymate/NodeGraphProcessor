using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor.EdgeProcessing;
using UnityEditor;
using UnityEngine.UIElements;
using static GraphProcessor.EdgeProcessing.EdgeProcessing;


namespace GraphProcessor
{
    [CustomPropertyDrawer(typeof(EdgeProcessOrderKey))]
    public class EdgeProcessOrderKeyPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty edgeProcessOrderKeyProperty)
        {
            var keyValueProperty = edgeProcessOrderKeyProperty.FindPropertyRelative(EdgeProcessOrderKey.ValueFieldName);

            string displayName = edgeProcessOrderKeyProperty.displayName;
            List<string> choices = EdgeProcessOrderBehaviorKeyValues.ToList();
            string currentValue = choices.Contains(keyValueProperty.stringValue) ? keyValueProperty.stringValue : EdgeProcessOrder.DefaultEdgeProcessOrder;

            var edgeProcessOrderField = new DropdownField(displayName, choices, currentValue);
            edgeProcessOrderField.RegisterValueChangedCallback((e) =>
            {
                keyValueProperty.stringValue = e.newValue;
                edgeProcessOrderKeyProperty.serializedObject.ApplyModifiedProperties();
            });

            return edgeProcessOrderField;
        }
    }
}