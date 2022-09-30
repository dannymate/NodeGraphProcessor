using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace GraphProcessor
{
    public class SubGraphOptionsGUIUtility
    {
        readonly SubGraph _subgraph;
        SerializedObject _subGraphSerialized;
        SerializedProperty _options;
        SerializedProperty _displayName;
        SerializedProperty _renamePolicy;

        public SubGraphOptionsGUIUtility(SubGraph subGraph)
        {
            this._subgraph = subGraph;
        }

        public SubGraph SubGraph => _subgraph;

        public SerializedObject SubGraphObject =>
            PropertyUtils.LazyLoad(
                ref _subGraphSerialized,
                () => new SerializedObject(SubGraph)
            );

        public SerializedProperty Options =>
            PropertyUtils.LazyLoad(
                ref _options,
                () => SubGraphObject.FindProperty(SubGraph.OptionsFieldName)
            );

        public SerializedProperty DisplayName =>
            PropertyUtils.LazyLoad(
                ref _displayName,
                () => Options.FindPropertyRelative(SubGraphOptions.DisplayNameFieldName)
            );

        public SerializedProperty RenamePolicy =>
            PropertyUtils.LazyLoad(
                ref _renamePolicy,
                () => Options.FindPropertyRelative(SubGraphOptions.RenamePolicyFieldName)
            );

        public Foldout DrawGUI()
        {
            var optionsFoldout = new Foldout()
            {
                text = "Options"
            };

            PropertyField displayNameField = DrawDisplayNameField(bind: false);
            displayNameField.RegisterCallback<ChangeEvent<string>>((prop) =>
            {
                if (string.Equals(prop.previousValue, prop.newValue))
                    return;

                SubGraph.NotifyOptionsChanged();
            });

            PropertyField renamePolicyField = DrawRenamePolicyField(bind: false);
            renamePolicyField.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                if (e.previousValue == e.newValue)
                    return;

                SubGraph.NotifyOptionsChanged();
            });

            optionsFoldout.Add(displayNameField);
            optionsFoldout.Add(renamePolicyField);

            optionsFoldout.Bind(SubGraphObject);

            return optionsFoldout;
        }

        public PropertyField DrawDisplayNameField(bool bind = true)
        {
            PropertyField displayNameField = new(DisplayName);

            if (bind) displayNameField.Bind(SubGraphObject);

            return displayNameField;
        }

        public PropertyField DrawRenamePolicyField(bool bind = true)
        {
            PropertyField renamePolicyField = new(RenamePolicy);

            if (bind) renamePolicyField.Bind(SubGraphObject);

            return renamePolicyField;
        }
    }
}