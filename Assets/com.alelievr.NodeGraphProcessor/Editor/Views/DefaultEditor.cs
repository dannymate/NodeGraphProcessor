#if UNITY_2022_1_OR_NEWER
#else
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Object), true, isFallback = true)]
public class DefaultEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    propertyField.SetEnabled(value: false);

                container.Add(propertyField);
            }
            while (iterator.NextVisible(false));
        }

        return container;
    }
}
#endif