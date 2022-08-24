using System.Collections;
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

    [MenuItem("Assets/Create/SerializedGraphProcessor", false, 10)]
    public static void CreateSerializedGraphProcessor()
    {
        var graph = ScriptableObject.CreateInstance<SerializedBaseGraph>();
        ProjectWindowUtil.CreateAsset(graph, "GraphProcessor.asset");
    }

    [OnOpenAsset(0)]
    public static bool OnBaseGraphOpened(int instanceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as GraphBase;

        if (asset != null && AssetDatabase.GetAssetPath(asset).Contains("Examples"))
        {
            EditorWindow.GetWindow<AllGraphWindow>().InitializeGraph(asset);
            return true;
        }
        return false;
    }
}
