using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor.Utils
{
    public static class ScriptableObjectUtils
    {
        /// <summary>
        /// Get all instances of scriptable objects with given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAllInstances<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<T>)
                        .ToList();
        }
    }
}