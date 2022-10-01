using System.Linq;
using GraphProcessor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace GraphProcessor
{
    [CustomPropertyDrawer(typeof(PortData))]
    public class PortDataDrawer : PropertyDrawer
    {
        private StyleBackground IdentifierTextBackground => (StyleBackground)EditorGUIUtility.IconContent("d_Text Icon").image;
        private StyleBackground IdentifierSOBackground => (StyleBackground)EditorGUIUtility.IconContent("d_ScriptableObject Icon").image;

        public override VisualElement CreatePropertyGUI(SerializedProperty portDataProperty)
        {
            SerializedProperty displayNameProperty = portDataProperty.FindPropertyRelative(PortData.DisplayNameFieldName);
            SerializedProperty displayTypeProperty = portDataProperty.FindPropertyRelative(PortData.DisplayTypeFieldName);
            SerializedProperty showAsDrawerProperty = portDataProperty.FindPropertyRelative(PortData.ShowAsDrawerFieldName);
            SerializedProperty acceptMultipleEdgesProperty = portDataProperty.FindPropertyRelative(PortData.AcceptMultipleEdgesFieldName);
            SerializedProperty tooltipProperty = portDataProperty.FindPropertyRelative(PortData.TooltipFieldName);
            SerializedProperty verticalProperty = portDataProperty.FindPropertyRelative(PortData.VerticalFieldName);

            Foldout container = new() { text = displayNameProperty.stringValue };

            // Create property fields.
            var identifierContainer = DrawIdentifierGUI(portDataProperty);
            var displayNameField = new PropertyField(displayNameProperty);
            var displayTypeField = new PropertyField(displayTypeProperty);
            var showAsDrawerField = new PropertyField(showAsDrawerProperty);
            var acceptMultipleEdgesField = new PropertyField(acceptMultipleEdgesProperty);
            var tooltipField = new PropertyField(tooltipProperty);
            var verticalField = new PropertyField(verticalProperty);

            displayNameField.RegisterCallback<ChangeEvent<string>>((e) => container.text = e.newValue);

            container.Add(identifierContainer);
            container.Add(displayNameField);
            container.Add(displayTypeField);
            container.Add(showAsDrawerField);
            container.Add(acceptMultipleEdgesField);
            container.Add(tooltipField);
            container.Add(verticalField);

            return container;
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
            identifierField.RegisterCallback<GeometryChangedEvent>((e) =>
            {
                // We do this because the indentation is wrong otherwise
                Label label = identifierField.Q<Label>();
                label.style.marginRight = -27;
                label.style.paddingRight = 30;
            });

            var identifierObjectField = new PropertyField(identifierObjectProperty);
            identifierObjectField.style.flexGrow = 1;
            identifierField.RegisterCallback<GeometryChangedEvent>((e) =>
            {
                // We do this because the indentation is wrong otherwise
                Label label = identifierObjectField.Q<Label>();
                label.style.marginRight = -27;
                label.style.paddingRight = 30;
            });

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