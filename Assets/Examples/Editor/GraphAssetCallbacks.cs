﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;
using System.IO;

public class GraphAssetCallbacks
{
    [MenuItem("Assets/Create/GraphProcessor", false, 10)]
    public static void CreateGraphProcessor()
    {
        var graph = ScriptableObject.CreateInstance<BaseGraph>();
        ProjectWindowUtil.CreateAsset(graph, "GraphProcessor.asset");
    }

    [OnOpenAsset(0)]
    public static bool OnBaseGraphOpened(int instanceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;

        if (asset != null && AssetDatabase.GetAssetPath(asset).Contains("Examples"))
        {
            EditorWindow.GetWindow<AllGraphWindow>().InitializeGraph(asset as BaseGraph);
            return true;
        }
        return false;
    }

    [MenuItem("Assets/Create/SubGraphProcessor", false, 11)]
    public static void CreateSubGraphProcessor()
    {
        var subGraph = ScriptableObject.CreateInstance<SubGraph>();
        ProjectWindowUtil.CreateAsset(subGraph, "SubGraphProcessor.asset");
    }
}
