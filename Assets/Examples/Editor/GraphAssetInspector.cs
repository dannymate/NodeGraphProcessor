using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEngine.UIElements;

[CustomEditor(typeof(GraphBase), true)]
public class GraphAssetInspector : GraphInspector
{
    // protected override void CreateInspector()
    // {
    // }

    protected override void CreateInspector()
    {
        base.CreateInspector();

        root.Add(new Button(() => EditorWindow.GetWindow<DefaultGraphWindow>().InitializeGraph(target as GraphBase))
        {
            text = "Open base graph window"
        });
        root.Add(new Button(() => EditorWindow.GetWindow<CustomContextMenuGraphWindow>().InitializeGraph(target as GraphBase))
        {
            text = "Open custom context menu graph window"
        });
        root.Add(new Button(() => EditorWindow.GetWindow<CustomToolbarGraphWindow>().InitializeGraph(target as GraphBase))
        {
            text = "Open custom toolbar graph window"
        });
        root.Add(new Button(() => EditorWindow.GetWindow<ExposedPropertiesGraphWindow>().InitializeGraph(target as GraphBase))
        {
            text = "Open exposed properties graph window"
        });
    }
}
