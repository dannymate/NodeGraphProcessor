using System;
using UnityEngine;

namespace GraphProcessor
{
    public static class NodeUtils
    {
        public delegate BaseNode NodeCreationMethod(Type nodeType, Vector2 position, params object[] args);
        public delegate BaseNode NodeCreationMethod<T>(Vector2 position, params object[] args);
    }
}