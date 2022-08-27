using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GraphProcessor
{
    /// <summary>
    /// Tell that this field is will generate an input port
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InputAttribute : Attribute
    {
        public string name;
        public bool allowMultiple = false;
        public bool showAsDrawer = false;

        /// <summary>
        /// Mark the field as an input port
        /// </summary>
        /// <param name="name">display name</param>
        /// <param name="allowMultiple">is connecting multiple edges allowed</param>
        public InputAttribute(string name = null, bool allowMultiple = false, bool showAsDrawer = false)
        {
            this.name = name;
            this.allowMultiple = allowMultiple;
            this.showAsDrawer = showAsDrawer;
        }
    }

    /// <summary>
    /// Tell that this field is will generate an output port
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class OutputAttribute : Attribute
    {
        public string name;
        public bool allowMultiple = true;

        /// <summary>
        /// Mark the field as an output port
        /// </summary>
        /// <param name="name">display name</param>
        /// <param name="allowMultiple">is connecting multiple edges allowed</param>
        public OutputAttribute(string name = null, bool allowMultiple = true)
        {
            this.name = name;
            this.allowMultiple = allowMultiple;
        }
    }

    /// <summary>
    /// Creates a vertical port instead of the default horizontal one
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class VerticalAttribute : Attribute
    {
    }

    /// <summary>
    /// Register the node in the NodeProvider class. The node will also be available in the node creation window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeMenuItemAttribute : Attribute
    {
        public string menuTitle;
        public Type onlyCompatibleWithGraph;

        /// <summary>
        /// Register the node in the NodeProvider class. The node will also be available in the node creation window.
        /// </summary>
        /// <param name="menuTitle">Path in the menu, use / as folder separators</param>
        public NodeMenuItemAttribute(string menuTitle = null, Type onlyCompatibleWithGraph = null)
        {
            this.menuTitle = menuTitle;
            this.onlyCompatibleWithGraph = onlyCompatibleWithGraph;
        }
    }

    /// <summary>
    /// Set a custom drawer for a field. It can then be created using the FieldFactory
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Obsolete("You can use the standard Unity CustomPropertyDrawer instead.")]
    public class FieldDrawerAttribute : Attribute
    {
        public Type fieldType;

        /// <summary>
        /// Register a custom view for a type in the FieldFactory class
        /// </summary>
        /// <param name="fieldType"></param>
        public FieldDrawerAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }

    /// <summary>
    /// Allow you to customize the input function of a port
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomPortInputAttribute : Attribute
    {
        public string fieldName;
        public Type inputType;
        public bool allowCast;

        /// <summary>
        /// Allow you to customize the input function of a port.
        /// See CustomPortsNode example in Samples.
        /// </summary>
        /// <param name="fieldName">local field of the node</param>
        /// <param name="inputType">type of input of the port</param>
        /// <param name="allowCast">if cast is allowed when connecting an edge</param>
        public CustomPortInputAttribute(string fieldName, Type inputType, bool allowCast = true)
        {
            this.fieldName = fieldName;
            this.inputType = inputType;
            this.allowCast = allowCast;
        }
    }

    /// <summary>
    /// Allow you to customize the input function of a port
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomPortOutputAttribute : Attribute
    {
        public string fieldName;
        public Type outputType;
        public bool allowCast;

        /// <summary>
        /// Allow you to customize the output function of a port.
        /// See CustomPortsNode example in Samples.
        /// </summary>
        /// <param name="fieldName">local field of the node</param>
        /// <param name="inputType">type of input of the port</param>
        /// <param name="allowCast">if cast is allowed when connecting an edge</param>
        public CustomPortOutputAttribute(string fieldName, Type outputType, bool allowCast = true)
        {
            this.fieldName = fieldName;
            this.outputType = outputType;
            this.allowCast = allowCast;
        }
    }

    /// <summary>
    /// Allow you to modify the generated port view from a field. Can be used to generate multiple ports from one field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomPortBehaviorAttribute : Attribute
    {
        public string fieldName;

        /// <summary>
        /// Allow you to modify the generated port view from a field. Can be used to generate multiple ports from one field.
        /// You must add this attribute on a function of this signature
        /// <code>
        /// IEnumerable&lt;PortData&gt; MyCustomPortFunction(List&lt;SerializableEdge&gt; edges);
        /// </code>
        /// </summary>
        /// <param name="fieldName">local node field name</param>
        public CustomPortBehaviorAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }

    /// <summary>
    /// Allow to bind a method to generate a specific set of ports based on a field type in a node
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomPortTypeBehavior : Attribute
    {
        /// <summary>
        /// Target type
        /// </summary>
        public Type type;

        public CustomPortTypeBehavior(Type type)
        {
            this.type = type;
        }
    }

    /// <summary>
    /// Allow you to have a custom view for your stack nodes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomStackNodeView : Attribute
    {
        public Type stackNodeType;

        /// <summary>
        /// Allow you to have a custom view for your stack nodes
        /// </summary>
        /// <param name="stackNodeType">The type of the stack node you target</param>
        public CustomStackNodeView(Type stackNodeType)
        {
            this.stackNodeType = stackNodeType;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class VisibleIf : Attribute
    {
        public string fieldName;
        public object value;

        public VisibleIf(string fieldName, object value)
        {
            this.fieldName = fieldName;
            this.value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowInInspector : Attribute
    {
        public bool showInNode;

        public ShowInInspector(bool showInNode = false)
        {
            this.showInNode = showInNode;
        }
    }

    // [Obsolete("ShowAsDrawer attribute is deprecated. Please use the InputAttribute showAsDrawer field.")]

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowAsDrawer : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        public string name;

        public SettingAttribute(string name = null)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// Marks an input/output to only be used with custom behaviour generated ports.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CustomBehaviourOnly : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class IsCompatibleWithGraph : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomMenuItem : Attribute
    {
        public string menuTitle;
        public object key;
        public object[] args;

        /// <summary>
        /// Register the node creation method in the NodeProvider class. The node creation method will also be available in the node creation window.
        /// </summary>
        /// <param name="menuTitle">Path in the menu, use / as folder separators</param>
        /// <param name"key">Key to check for with other attributes like CustomMenuItemFilter</param>
        public CustomMenuItem(string menuTitle = null, object key = null)
        {
            this.menuTitle = menuTitle;
            this.key = key;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomMenuItemFilter : Attribute
    {
        public string portFieldName;
        public object key;
        public Type type;

        /// <summary>
        /// Used alongside CustomMenuItemAttribute to filter a certain Port to a Type. Can only be used to remove NodeTypes from the Menu.
        /// If the NodeType doesn't show without the filter then this filter will do nothing.
        /// Works with both CustomClassMenuItem and CustomMenuItem.
        /// </summary>
        /// <param name="portFieldName">FieldName of the port to filter.</param>
        /// <param name="type">Type to Check For</param>
        /// <param name"key">Key to check for with other attributes like CustomMenuItem</param>
        public CustomMenuItemFilter(string portFieldName, Type type, object key = null)
        {
            this.portFieldName = portFieldName;
            this.type = type;
            this.key = key;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class CustomClassMenuItem : CustomMenuItem
    {
        public Type nodeType;

        public Type methodParentClass;
        public string methodName;

        public bool IsBasic => String.IsNullOrEmpty(methodName);

        /// <summary>
        /// Automate registration of a type declaration in the NodeProvider and creation window.
        /// </summary>
        /// <param name="menuTitle">Path in the menu, use / as folder separators</param>
        /// <param name="nodeType">The node to spawn.</param>
        /// <param name"key">Key to check for with other attributes like CustomMenuItemFilter</param>
        public CustomClassMenuItem(string menuTitle, Type nodeType, object key = null)
        {
            this.menuTitle = menuTitle;
            this.nodeType = nodeType;
            this.key = key;
        }

        /// <summary>
        /// Automate registration of a type declaration in the NodeProvider and creation window.
        /// </summary>
        /// <param name="menuTitle">Path in the menu, use / as folder separators</param>
        /// <param name="methodParentClass">The class that contains methodName</param>
        /// <param name="methodName">The name of the public static method that creates the node</param>
        /// <param name"key">Key to check for with other attributes like CustomMenuItemFilter</param>
        /// <param name="args">Extra args will get sent to the specified method allowing the same method to produce different results</param>
        public CustomClassMenuItem(string menuTitle, Type methodParentClass, string methodName, object key = null, params object[] args)
        {
            this.menuTitle = menuTitle;
            this.methodParentClass = methodParentClass;
            this.methodName = methodName;
            this.key = key;
            this.args = args;
        }
    }
}