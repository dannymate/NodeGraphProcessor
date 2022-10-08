using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;

namespace GraphProcessor
{
    public class SubGraphWindow : BaseGraphWindow
    {
        protected override void OnDestroy()
        {
            graphView?.Dispose();
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("Default Graph");

            if (graphView == null)
                graphView = new BaseGraphView(this);

            rootView.Add(graphView);
        }
    }
}