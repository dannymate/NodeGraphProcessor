using System.Reflection;
using static GraphProcessor.NodeDelegates;
using System;
using UnityEngine;
using GraphProcessor.EdgeProcessing;

namespace GraphProcessor
{
    public abstract partial class BaseNode
    {
        public class NodeFieldInformation
        {
            public string name;
            public string fieldName;
            public UnityPath proxiedFieldPath;
            public object memberOwner;
            public MemberInfo info;
            public bool input;
            public bool isMultiple;
            public EdgeProcessOrderKey processOrder;
            public Type displayType;
            public string tooltip;
            public bool showAsDrawer;
            public CustomPortBehaviorDelegateInfo behavior;
            public bool vertical;

            public NodeFieldInformation(object memberOwner, MemberInfo info, CustomPortBehaviorDelegateInfo behavior, UnityPath proxiedFieldPath = null)
            {
                var inputAttribute = info.GetCustomAttribute<InputAttribute>();
                var outputAttribute = info.GetCustomAttribute<OutputAttribute>();
                var tooltipAttribute = info.GetCustomAttribute<TooltipAttribute>();

                string name = info.Name;
                if (!string.IsNullOrEmpty(inputAttribute?.name))
                    name = inputAttribute.name;
                if (!string.IsNullOrEmpty(outputAttribute?.name))
                    name = outputAttribute.name;

                this.memberOwner = memberOwner;
                this.input = inputAttribute != null;
                this.isMultiple = (inputAttribute != null) ? inputAttribute.AcceptsMultipleEdges : outputAttribute.allowMultiple;
                this.info = info;
                this.name = name;
                this.fieldName = info.Name;
                this.proxiedFieldPath = proxiedFieldPath;
                this.displayType = inputAttribute?.displayType;
                this.processOrder = (inputAttribute as MultiEdgeInputAttribute)?.processOrder ?? EdgeProcessOrder.DefaultEdgeProcessOrder;
                this.behavior = behavior;
                this.tooltip = tooltipAttribute?.tooltip;
                this.showAsDrawer = input && inputAttribute.showAsDrawer;
                this.vertical = info.HasCustomAttribute<VerticalAttribute>();
            }
        }
    }
}

