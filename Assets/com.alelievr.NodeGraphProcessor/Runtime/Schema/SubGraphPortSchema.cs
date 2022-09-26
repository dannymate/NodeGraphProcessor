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

        public VisualElement DrawControlGUI()
        {
            var root = new VisualElement();

            root.Add(DrawInputDataGUI(bind: false));
            root.Add(DrawOutputDataGUI(bind: false));
            root.Add(DrawUpdateSchemaButtonGUI());

            root.Bind(ThisSerialized);

            return root;
        }

        public PropertyField DrawInputDataGUI(bool bind = true)
        {
            var inputData = new PropertyField(InputDataSerialized);
            if (bind) inputData.Bind(ThisSerialized);
            return inputData;
        }

        public PropertyField DrawOutputDataGUI(bool bind = true)
        {
            var outputData = new PropertyField(OutputDataSerialized);
            if (bind) outputData.Bind(ThisSerialized);
            return outputData;
        }

        public Button DrawUpdateSchemaButtonGUI()
        {
            var updateSchemaButton = new Button(() => NotifyPortsChanged()) { text = "UPDATE SCHEMA" };
            return updateSchemaButton;
        }
    }
}