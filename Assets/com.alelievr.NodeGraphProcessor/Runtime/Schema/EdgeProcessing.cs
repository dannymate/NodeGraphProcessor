using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor.EdgeProcessing
{
    public static class EdgeProcessing
    {
        private static Dictionary<EdgeProcessOrderKey, EdgeProcessOrderCallback> _edgeProcessOrderCallbackByKey;

        public delegate IList<SerializableEdge> EdgeProcessOrderCallback(IList<SerializableEdge> edges);

        public static Dictionary<EdgeProcessOrderKey, EdgeProcessOrderCallback> EdgeProcessOrderCallbackByKey =>
            PropertyUtils.LazyLoad(ref _edgeProcessOrderCallbackByKey, BuildEdgeProcessOrderBehaviorDict);

        public static EdgeProcessOrderKey[] EdgeProcessOrderBehaviorKeys => EdgeProcessOrderCallbackByKey.Keys.ToArray();

        private static Dictionary<EdgeProcessOrderKey, EdgeProcessOrderCallback> BuildEdgeProcessOrderBehaviorDict()
        {
            Dictionary<EdgeProcessOrderKey, EdgeProcessOrderCallback> edgeProcessOrderByName = new();

            foreach (var methodInfo in TypeCache.GetMethodsWithAttribute<EdgeOrdererAttribute>())
            {
                if (!methodInfo.HasCustomAttribute<EdgeOrdererAttribute>()) continue;

                EdgeOrdererAttribute attribute = methodInfo.GetCustomAttribute<EdgeOrdererAttribute>();

                if (edgeProcessOrderByName.ContainsKey(attribute.Key))
                {
                    Debug.LogError("Edge Ordering Method with Key: " + attribute.Key + " already exists. SKIPPING!");
                    continue;
                }

                edgeProcessOrderByName.Add(attribute.Key, methodInfo.CreateDelegate(typeof(EdgeProcessOrderCallback)) as EdgeProcessOrderCallback);
            }

            return edgeProcessOrderByName;
        }
    }
}