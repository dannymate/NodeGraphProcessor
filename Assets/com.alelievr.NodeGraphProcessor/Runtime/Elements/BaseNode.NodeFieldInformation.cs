using System.Reflection;
using static GraphProcessor.NodeDelegates;
using System;
using UnityEngine;
using GraphProcessor.EdgeProcessing;

namespace GraphProcessor
{
    public abstract partial class BaseNode
    {
        internal class NodeFieldInformation
        {
            public string name;
            public string fieldName;
            public MemberInfo info;
            public bool input;
            public bool isMultiple;
            public EdgeProcessOrderKey processOrder;
            public Type displayType;
            public string tooltip;
            public bool showAsDrawer;
            public CustomPortBehaviorDelegateInfo behavior;
            public bool vertical;

            public NodeFieldInformation(MemberInfo info)
            {
                var inputAttribute = info.GetCustomAttribute<InputAttribute>();
                var outputAttribute = info.GetCustomAttribute<OutputAttribute>();
                var tooltipAttribute = info.GetCustomAttribute<TooltipAttribute>();
                bool isMultiple;
                bool isInput;
                string name = info.Name;
                string tooltip;
                bool showAsDrawer = false;

                isInput = inputAttribute != null;
                isMultiple = (inputAttribute != null) ? inputAttribute.AcceptsMultipleEdges : outputAttribute.allowMultiple;

                if (isInput)
                    showAsDrawer = inputAttribute.showAsDrawer;

                tooltip = tooltipAttribute?.tooltip;

                if (!string.IsNullOrEmpty(inputAttribute?.name))
                    name = inputAttribute.name;
                if (!string.IsNullOrEmpty(outputAttribute?.name))
                    name = outputAttribute.name;


                this.input = isInput;
                this.isMultiple = isMultiple;
                this.info = info;
                this.name = name;
                this.fieldName = info.Name;
                this.displayType = (inputAttribute as MultiEdgeInputAttribute)?.displayType;
                this.processOrder = (inputAttribute as MultiEdgeInputAttribute)?.processOrder ?? EdgeProcessOrder.FIFO;
                this.behavior = null; // Set after instantiation
                this.tooltip = tooltip;
                this.showAsDrawer = showAsDrawer;
                this.vertical = info.HasCustomAttribute<VerticalAttribute>();
            }
        }
    }
}

