using UnityEngine;
using GraphProcessor;

[ExecuteAlways]
public class GraphBehaviour : MonoBehaviour
{
    public GraphBase graph;

    protected virtual void OnEnable()
    {
        if (graph == null)
            graph = ScriptableObject.CreateInstance<GraphBase>();

        graph.LinkToScene(gameObject.scene);
    }
}
