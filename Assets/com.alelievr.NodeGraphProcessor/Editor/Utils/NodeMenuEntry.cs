using UnityEngine;
using System;
using static GraphProcessor.NodeUtils;
using System.Reflection;

namespace GraphProcessor
{
    public static partial class NodeProvider
    {
        public class NodeMenuEntry : IEquatable<NodeMenuEntry>
        {

            public NodeMenuEntry(string path, Type nodeType, NodeCreationMethod creationMethod, object[] creationMethodArgs)
            {
                this.Path = path;
                this.NodeType = nodeType;
                this.CreationMethod = creationMethod;
                this.CreationMethodArgs = creationMethodArgs;
            }

            public string Path { get; }
            public Type NodeType { get; }
            public NodeCreationMethod CreationMethod { get; }
            public object[] CreationMethodArgs { get; }

            public bool Equals(NodeMenuEntry other)
            {
                if (!String.Equals(Path, other.Path)) return false;
                if (!Type.Equals(NodeType, other.NodeType)) return false;
                // Leave out CreationMethod and CreationMethodArgs because we only care about the above
                return true;
            }
        }

        public class NodeMenuEntryMethod
        {
            public NodeMenuEntryMethod(MethodInfo methodInfo, CustomMenuItem context, CustomMenuItemFilter[] filters)
            {
                MethodInfo = methodInfo;
                Context = context;
                Filters = filters;
            }

            public MethodInfo MethodInfo { get; }
            public CustomMenuItem Context { get; }
            public CustomMenuItemFilter[] Filters { get; }
        }
    }
}
