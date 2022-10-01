using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CustomPropertyDrawer(typeof(PortData))]
    public class PortDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty identifierProperty = property.FindPropertyRelative(PortData.IdentifierFieldName);
            SerializedProperty identifierObjectProperty = property.FindPropertyRelative(PortData.IdentifierObjectFieldName);
            SerializedProperty useIdentifierObjectProperty = property.FindPropertyRelative(PortData.UseIdentifierObjectFieldName);
            SerializedProperty displayNameProperty = property.FindPropertyRelative(PortData.DisplayNameFieldName);
            SerializedProperty displayTypeProperty = property.FindPropertyRelative(PortData.DisplayTypeFieldName);
            SerializedProperty showAsDrawerProperty = property.FindPropertyRelative(PortData.ShowAsDrawerFieldName);
            SerializedProperty acceptMultipleEdgesProperty = property.FindPropertyRelative(PortData.AcceptMultipleEdgesFieldName);
            SerializedProperty tooltipProperty = property.FindPropertyRelative(PortData.TooltipFieldName);
            SerializedProperty verticalProperty = property.FindPropertyRelative(PortData.VerticalFieldName);


            Foldout container = new()
            {
                text = displayNameProperty.stringValue,
            };

            // Create property fields.
            var identifierField = new PropertyField(identifierProperty);
            var identifierObjectField = new PropertyField(identifierObjectProperty);
            var useIdentifierObjectField = new PropertyField(useIdentifierObjectProperty);
            var displayNameField = new PropertyField(displayNameProperty);
            var displayTypeField = new PropertyField(displayTypeProperty);
            var showAsDrawerField = new PropertyField(showAsDrawerProperty);
            var acceptMultipleEdgesField = new PropertyField(acceptMultipleEdgesProperty);
            var tooltipField = new PropertyField(tooltipProperty);
            var verticalField = new PropertyField(verticalProperty);

            displayNameField.RegisterValueChangeCallback((e) =>
            {
                container.text = displayNameProperty.stringValue;
            });

            container.Add(identifierField);
            container.Add(identifierObjectField);
            container.Add(useIdentifierObjectField);
            container.Add(displayNameField);
            container.Add(displayTypeField);
            container.Add(showAsDrawerField);
            container.Add(acceptMultipleEdgesField);
            container.Add(tooltipField);
            container.Add(verticalField);

            return container;
        }
    }
}