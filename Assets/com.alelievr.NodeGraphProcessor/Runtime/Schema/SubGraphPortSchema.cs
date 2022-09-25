using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [CreateAssetMenu(fileName = "SubGraphPortSchema", menuName = "NGP/Schema/SubGraphPortSchema", order = 0)]
    public class SubGraphPortSchema : ScriptableObject
    {
        public event Notify OnPortsUpdated;

        [SerializeField]
        public List<PortData> inputData;

        [SerializeField]
        public List<PortData> outputData;

        SerializedObject _thisSerialized;
        public SerializedObject ThisSerialized
        {
            get
            {
                if (_thisSerialized == null)
                {
                    _thisSerialized = new SerializedObject(this);
                }
                return _thisSerialized;
            }
        }

        SerializedProperty _inputDataSerialized;
        public SerializedProperty InputDataSerialized
        {
            get
            {
                if (_inputDataSerialized == null)
                {
                    _inputDataSerialized = ThisSerialized.FindProperty(nameof(inputData));
                }
                return _inputDataSerialized;
            }
        }

        SerializedProperty _outputDataSerialized;
        public SerializedProperty OutputDataSerialized
        {
            get
            {
                if (_outputDataSerialized == null)
                {
                    _outputDataSerialized = ThisSerialized.FindProperty(nameof(outputData));
                }
                return _outputDataSerialized;
            }
        }

        public void NotifyPortsChanged()
        {
            OnPortsUpdated?.Invoke();
        }

        public void AddUpdatePortsListener(Notify listener)
        {
            OnPortsUpdated += listener;
        }

        public void RemoveUpdatePortsListener(Notify listener)
        {
            OnPortsUpdated -= listener;
        }

        public void DrawControlGUI(VisualElement root)
        {
            DrawInputDataGUI(root);
            DrawOutputDataGUI(root);
            DrawUpdateSchemaButtonGUI(root);
        }

        public void DrawInputDataGUI(VisualElement root)
        {
            VisualElement inputData = new PropertyField(InputDataSerialized);
            inputData.Bind(ThisSerialized);
            root.Add(inputData);
        }

        public void DrawOutputDataGUI(VisualElement root)
        {
            VisualElement outputData = new PropertyField(OutputDataSerialized);
            outputData.Bind(ThisSerialized);
            root.Add(outputData);
        }

        public void DrawUpdateSchemaButtonGUI(VisualElement root)
        {
            VisualElement updatePortsButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE SCHEMA" };
            root.Add(updatePortsButton);
        }
    }
}