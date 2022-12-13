using System.Linq;
using GraphProcessor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using static GraphProcessor.EdgeProcessing.EdgeProcessing;
using System;

namespace GraphProcessor
{
    [CustomPropertyDrawer(typeof(PortData))]
    public class PortDataDrawer : PropertyDrawer
    {
        public static Action<VisualElement, GeometryChangedEvent> FixBrokenIndentation => (ve, e) =>
            {
                // We do this because the indentation is wrong otherwise
                Label label = ve.Q<Label>();
                label.style.marginRight = -27;
                label.style.paddingRight = 30;
            };

        private StyleBackground IdentifierTextBackground => (StyleBackground)EditorGUIUtility.IconContent("d_Text Icon").image;
        private StyleBackground IdentifierSOBackground => (StyleBackground)EditorGUIUtility.IconContent("d_ScriptableObject Icon").image;

        public override VisualElement CreatePropertyGUI(SerializedProperty portDataProperty)
        {

            SerializedProperty displayNameProperty = portDataProperty.FindPropertyRelative(PortData.DisplayNameFieldName);
            SerializedProperty displayTypeProperty = portDataProperty.FindPropertyRelative(PortData.DisplayTypeFieldName);
            SerializedProperty showAsDrawerProperty = portDataProperty.FindPropertyRelative(PortData.ShowAsDrawerFieldName);
            SerializedProperty tooltipProperty = portDataProperty.FindPropertyRelative(PortData.TooltipFieldName);
            SerializedProperty verticalProperty = portDataProperty.FindPropertyRelative(PortData.VerticalFieldName);

            Foldout container = new() { text = displayNameProperty.stringValue };

            // Create property fields.
            var identifierContainer = DrawIdentifierGUI(portDataProperty);
            var displayNameField = new PropertyField(displayNameProperty);
            var displayTypeField = new PropertyField(displayTypeProperty);
            var showAsDrawerField = new PropertyField(showAsDrawerProperty);
            var multiEdgeContainer = DrawMultiEdgeGUI(portDataProperty);
            var tooltipField = new PropertyField(tooltipProperty);
            var verticalField = new PropertyField(verticalProperty);

            displayNameField.RegisterCallback<ChangeEvent<string>>((e) => container.text = e.newValue);

            container.Add(identifierContainer);
            container.Add(displayNameField);
            container.Add(displayTypeField);
            container.Add(showAsDrawerField);
            container.Add(multiEdgeContainer);
            container.Add(tooltipField);
            container.Add(verticalField);

            return container;
        }

        private VisualElement DrawMultiEdgeGUI(SerializedProperty portDataProperty)
        {
            SerializedProperty acceptMultipleEdgesProperty = portDataProperty.FindPropertyRelative(PortData.AcceptMultipleEdgesFieldName);
            SerializedProperty edgeProcessOrderProperty = portDataProperty.FindPropertyRelative(PortData.EdgeProcessOrderFieldName);

            VisualElement multiEdgeContainer = new();

            var acceptMultipleEdgesField = new PropertyField(acceptMultipleEdgesProperty);
            var edgeProcessOrderField = new PropertyField(edgeProcessOrderProperty);

            acceptMultipleEdgesField.RegisterCallback<GeometryChangedEvent>((e) => FixBrokenIndentation(acceptMultipleEdgesField, e));
            acceptMultipleEdgesField.RegisterValueChangeCallback((e) =>
            {
                SetEdgeProcessOrderFieldDisplay(acceptMultipleEdgesProperty.boolValue);
            });

            SetEdgeProcessOrderFieldDisplay(acceptMultipleEdgesProperty.boolValue);

            multiEdgeContainer.Add(acceptMultipleEdgesField);
            multiEdgeContainer.Add(edgeProcessOrderField);

            return multiEdgeContainer;

            void SetEdgeProcessOrderFieldDisplay(bool show)
            {
                if (acceptMultipleEdgesProperty.boolValue)
                    edgeProcessOrderField.Show();
                else
                    edgeProcessOrderField.Hide();
            }
        }

        private VisualElement DrawIdentifierGUI(SerializedProperty portDataProperty)
        {
            SerializedProperty identifierProperty = portDataProperty.FindPropertyRelative(PortData.IdentifierFieldName);
            SerializedProperty identifierObjectProperty = portDataProperty.FindPropertyRelative(PortData.IdentifierObjectFieldName);
            SerializedProperty useIdentifierObjectProperty = portDataProperty.FindPropertyRelative(PortData.UseIdentifierObjectFieldName);

            VisualElement identifierContainer = new();
            identifierContainer.style.flexDirection = FlexDirection.Row;

            var identifierField = new PropertyField(identifierProperty);
            identifierField.style.flexGrow = 1;
            identifierField.RegisterCallback<GeometryChangedEvent>((e) => FixBrokenIndentation(identifierField, e));

            var identifierObjectField = new PropertyField(identifierObjectProperty);
            identifierObjectField.style.flexGrow = 1;
            identifierObjectField.RegisterCallback<GeometryChangedEvent>((e) => FixBrokenIndentation(identifierObjectField, e));

            var useIdentifierObjectButton = new Button().SetSize(14, 14).SetMargin(0, 0, 8, 2);
            useIdentifierObjectButton.clicked += () =>
            {
                bool useIdentifierObject = !useIdentifierObjectProperty.boolValue;

                useIdentifierObjectProperty.boolValue = useIdentifierObject;
                useIdentifierObjectProperty.serializedObject.ApplyModifiedProperties();

                SetIdentifierStyle(useIdentifierObject);
            };

            SetIdentifierStyle(useIdentifierObjectProperty.boolValue);

            identifierContainer.Add(identifierField);
            identifierContainer.Add(identifierObjectField);
            identifierContainer.Add(useIdentifierObjectButton);

            return identifierContainer;

            void SetIdentifierStyle(bool useIdentifierObject)
            {
                if (!useIdentifierObject)
                {
                    identifierField.Show();
                    identifierObjectField.Hide();
                    useIdentifierObjectButton.style.backgroundImage = IdentifierSOBackground;
                }
                else
                {
                    identifierField.Hide();
                    identifierObjectField.Show();
                    useIdentifierObjectButton.style.backgroundImage = IdentifierTextBackground;
                }

            }
        }
    }
}