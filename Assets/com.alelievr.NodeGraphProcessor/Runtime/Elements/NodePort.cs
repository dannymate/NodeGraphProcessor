using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Linq.Expressions;
using System;
using TypeReferences;
using GraphProcessor.EdgeProcessing;
using static GraphProcessor.EdgeProcessing.EdgeProcessing;
using static GraphProcessor.BaseNode;

namespace GraphProcessor
{
    /// <summary>
    /// Class that describe port attributes for it's creation
    /// </summary>
    [Serializable]
    public class PortData : IEquatable<PortData>, ICloneable
    {
        public const string IdentifierFieldName = nameof(identifier);
        public const string IdentifierObjectFieldName = nameof(identifierObject);
        public const string UseIdentifierObjectFieldName = nameof(useIdentifierObject);
        public const string DisplayNameFieldName = nameof(displayName);
        public const string DisplayTypeFieldName = nameof(displayType);
        public const string ShowAsDrawerFieldName = nameof(showAsDrawer);
        public const string AcceptMultipleEdgesFieldName = nameof(acceptMultipleEdges);
        public const string EdgeProcessOrderFieldName = nameof(edgeProcessOrder);
        public const string TooltipFieldName = nameof(tooltip);
        public const string VerticalFieldName = nameof(vertical);

        /// <summary>
        /// Unique identifier for the port
        /// </summary>
        [SerializeField]
        public string identifier;

        [SerializeField]
        private PortIdentifier identifierObject;

        [SerializeField]
        public bool useIdentifierObject = false;

        public string Identifier => useIdentifierObject && identifierObject ? identifierObject : identifier;

        /// <summary>
        /// Display name on the node
        /// </summary>
        [SerializeField]
        public string displayName;
        /// <summary>
        /// The type that will be used for coloring with the type stylesheet
        /// </summary>
        [SerializeField, TypeOptions(ShowAllTypes = true)]
        public TypeReference displayType = new();
        /// <summary>
        /// Whether to show a property drawer with this port (only for input)
        /// </summary>
        [SerializeField]
        public bool showAsDrawer;
        /// <summary>
        /// If the port accept multiple connection
        /// </summary>
        [SerializeField]
        public bool acceptMultipleEdges;
        /// <summary>
        /// Order to process connected edges
        /// </summary>
        [SerializeField]
        public EdgeProcessOrderKey edgeProcessOrder = EdgeProcessOrder.DefaultEdgeProcessOrder;
        /// <summary>
        /// The field the port is proxying if using custombehavior
        /// </summary>
        [SerializeField, HideInInspector]
        public UnityPath proxiedFieldPath;
        /// <summary>
        /// Port size, will also affect the size of the connected edge
        /// </summary>
        [SerializeField, HideInInspector]
        public int sizeInPixel;
        /// <summary>
        /// Tooltip of the port
        /// </summary>
        [SerializeField]
        public string tooltip;
        /// <summary>
        /// Is the port vertical
        /// </summary>
        [SerializeField]
        public bool vertical;

        // Need to decide whether to switch over the properties.
        #region SwitchToProperties
        /// Replace "set" with "init" when Unity supports https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#init-only-setters
        // public string Identifier { get => identifier; set => identifier = value; }
        // public string DisplayName { get => displayName; set => displayName = value; }
        public Type DisplayType { get => displayType.Type; set => displayType = value; }
        // public bool ShowAsDrawer { get => showAsDrawer; set => showAsDrawer = value; }
        // public bool AcceptMultipleEdges { get => acceptMultipleEdges; set => acceptMultipleEdges = value; }
        // public string ProxiedFieldPath { get => proxiedFieldPath; set => proxiedFieldPath = value; }
        // public int SizeInPixel { get => sizeInPixel; set => sizeInPixel = value; }
        // public string Tooltip { get => tooltip; set => tooltip = value; }
        // public bool Vertical { get => vertical; set => vertical = value; }
        #endregion

        public bool IsProxied => proxiedFieldPath != null;

        public bool Equals(PortData other)
        {
            return other != null
                && Identifier == other.Identifier
                && displayName == other.displayName
                && DisplayType == other.DisplayType
                && showAsDrawer == other.showAsDrawer
                && acceptMultipleEdges == other.acceptMultipleEdges
                && edgeProcessOrder == other.edgeProcessOrder
                && sizeInPixel == other.sizeInPixel
                && proxiedFieldPath == other.proxiedFieldPath
                && tooltip == other.tooltip
                && vertical == other.vertical;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as PortData);
        }

        public override int GetHashCode()
        {
            return Identifier?.GetHashCode() ?? 0;
        }

        public void CopyFrom(PortData other)
        {
            identifier = other.Identifier;
            displayName = other.displayName;
            displayType = other.displayType;
            showAsDrawer = other.showAsDrawer;
            acceptMultipleEdges = other.acceptMultipleEdges;
            edgeProcessOrder = other.edgeProcessOrder;
            sizeInPixel = other.sizeInPixel;
            proxiedFieldPath = other.proxiedFieldPath;
            tooltip = other.tooltip;
            vertical = other.vertical;
        }

        public object Clone()
        {
            PortData portData = this.MemberwiseClone() as PortData;
            portData.displayType = new TypeReference(displayType);
            return portData;
        }

        public static bool operator ==(PortData lhs, PortData rhs)
        {
            if (lhs is null && rhs is null) return true;
            else if (lhs is null || rhs is null) return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(PortData lhs, PortData rhs)
        {
            return !(lhs == rhs);
        }
    }

    /// <summary>
    /// Runtime class that stores all info about one port that is needed for the processing
    /// </summary>
    public class NodePort
    {
        /// <summary>
        /// The actual name of the property behind the port (must be exact, it is used for Reflection)
        /// </summary>
        public string fieldName;
        /// <summary>
        /// The node on which the port is
        /// </summary>
        public BaseNode owner;
        /// <summary>
        /// The fieldInfo from the fieldName
        /// </summary>
        public MemberInfo fieldInfo;
        /// <summary>
        /// Data of the port
        /// </summary>
        public PortData portData;
        List<SerializableEdge> edges = new List<SerializableEdge>();
        Dictionary<SerializableEdge, PushDataDelegate> pushDataDelegates = new Dictionary<SerializableEdge, PushDataDelegate>();
        List<SerializableEdge> edgeWithRemoteCustomIO = new List<SerializableEdge>();
        List<SerializableEdge> edgeWithArrayInput = new();

        public bool IsInput => owner.inputPorts.Contains(this);
        public bool IsMultiEdgeInput => IsInput && portData.acceptMultipleEdges;

        /// <summary>
        /// Owner of the FieldInfo, to be used in case of Get/SetValue
        /// </summary>
        public object fieldOwner;

        CustomPortIODelegate customPortIOMethod;

        /// <summary>
        /// Delegate that is made to send the data from this port to another port connected through an edge
        /// This is an optimization compared to dynamically setting values using Reflection (which is really slow)
        /// More info: https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
        /// </summary>
        public delegate void PushDataDelegate();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">owner node</param>
        /// <param name="fieldName">the C# property name</param>
        /// <param name="portData">Data of the port</param>
        public NodePort(BaseNode owner, string fieldName, PortData portData) : this(owner, owner, fieldName, portData) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">owner node</param>
        /// <param name="fieldOwner"></param>
        /// <param name="fieldName">the C# property name</param>
        /// <param name="portData">Data of the port</param>
        public NodePort(BaseNode owner, object fieldOwner, string fieldName, PortData portData)
        {
            this.fieldName = fieldName;
            this.owner = owner;
            this.portData = portData;
            this.fieldOwner = fieldOwner;

            fieldInfo = fieldOwner.GetType().GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                fieldInfo = fieldOwner.GetType().GetProperty(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            customPortIOMethod = CustomPortIO.GetCustomPortMethod(owner.GetType(), fieldName);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">owner node</param>
        /// <param name="nodeFieldInformation"></param>
        /// <param name="portData">Data of the port</param>
        public NodePort(BaseNode owner, NodeFieldInformation nodeFieldInformation, PortData portData)
        {
            this.fieldName = nodeFieldInformation.fieldName;
            this.owner = owner;
            this.portData = portData;
            this.fieldOwner = nodeFieldInformation.memberOwner;
            this.fieldInfo = nodeFieldInformation.info;

            customPortIOMethod = CustomPortIO.GetCustomPortMethod(owner.GetType(), fieldName);
        }

        /// <summary>
        /// Connect an edge to this port
        /// </summary>
        /// <param name="edge"></param>
        public void Add(SerializableEdge edge)
        {
            if (!edges.Contains(edge))
                edges.Add(edge);

            if (edge.inputNode == owner)
            {
                if (edge.outputPort.customPortIOMethod != null)
                    edgeWithRemoteCustomIO.Add(edge);
            }
            else
            {
                if (edge.inputPort.customPortIOMethod != null)
                    edgeWithRemoteCustomIO.Add(edge);
            }

            //if we have a custom io implementation, we don't need to genereate the defaut one
            if (edge.inputPort.customPortIOMethod != null || edge.outputPort.customPortIOMethod != null)
                return;

            // We want to process all edges at once and provide a list
            if (edge.inputPort.IsMultiEdgeInput)
            {
                edgeWithArrayInput.Add(edge);
                return;
            }

            PushDataDelegate edgeDelegate = CreatePushDataDelegateForEdge(edge);

            if (edgeDelegate != null)
                pushDataDelegates[edge] = edgeDelegate;
        }

        PushDataDelegate CreatePushDataDelegateForEdge(SerializableEdge edge)
        {
            try
            {
                //Creation of the delegate to move the data from the input node to the output node:
                MemberInfo inputField = edge.inputPort.fieldInfo;
                MemberInfo outputField = edge.outputPort.fieldInfo;
                Type inType, outType;

#if DEBUG_LAMBDA
				return new PushDataDelegate(() => {
					var outValue = outputField.GetValue(edge.outputNode);
					inType = edge.inputPort.portData.displayType ?? inputField.FieldType;
					outType = edge.outputPort.portData.displayType ?? outputField.FieldType;
					Debug.Log($"Push: {inType}({outValue}) -> {outType} | {owner.name}");

					object convertedValue = outValue;
					if (TypeAdapter.AreAssignable(outType, inType))
					{
						var conversionMethod = TypeAdapter.GetConversionMethod(outType, inType);
						Debug.Log("Conversion method: " + conversionMethod.Name);
						convertedValue = conversionMethod.Invoke(null, new object[]{ outValue });
					}

					inputField.SetValue(edge.inputNode, convertedValue);
				});
#endif

                // We keep slow checks inside the editor
#if UNITY_EDITOR
                if (!BaseGraph.TypesAreConnectable(inputField.GetUnderlyingType(), outputField.GetUnderlyingType()))
                {
                    Debug.LogError("Can't convert from " + inputField.GetUnderlyingType() + " to " + outputField.GetUnderlyingType() + ", you must specify a custom port function (i.e CustomPortInput or CustomPortOutput) for non-implicit conversions");
                    return null;
                }
#endif

                Expression inputParamField = Expression.PropertyOrField(Expression.Constant(edge.inputPort.fieldOwner), inputField.Name);
                Expression outputParamField = Expression.PropertyOrField(Expression.Constant(edge.outputPort.fieldOwner), outputField.Name);

                inType = edge.inputPort.portData.DisplayType ?? inputField.GetUnderlyingType();
                outType = edge.outputPort.portData.DisplayType ?? outputField.GetUnderlyingType();

                // If there is a user defined conversion function, then we call it
                if (TypeAdapter.AreAssignable(outType, inType))
                {
                    // We add a cast in case there we're calling the conversion method with a base class parameter (like object)
                    var convertedParam = Expression.Convert(outputParamField, outType);
                    outputParamField = Expression.Call(TypeAdapter.GetConversionMethod(outType, inType), convertedParam);
                    // In case there is a custom port behavior in the output, then we need to re-cast to the base type because
                    // the conversion method return type is not always assignable directly:
                    outputParamField = Expression.Convert(outputParamField, inputField.GetUnderlyingType());
                }
                else // otherwise we cast
                    outputParamField = Expression.Convert(outputParamField, inputField.GetUnderlyingType());

                BinaryExpression assign = Expression.Assign(inputParamField, outputParamField);
                return Expression.Lambda<PushDataDelegate>(assign).Compile();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        PushDataDelegate CreatePushDataDelegateForMultiEdgeInput(IList<SerializableEdge> edges)
        {
            static string IsNotInputErrorMessage() => $"{nameof(CreatePushDataDelegateForMultiEdgeInput)} should only be called within an input!";
            static string IsNotCollectionTypeErrorMessage(Type inType) => $"{inType} is not a collection type!";
            static string UnexpectedElementTypeErrorMessage(SerializableEdge edge, Type expectedElementType)
            {
                string outputNodeName = edge.outputNode.GetCustomName();
                string outputPortName = edge.outputPort.portData.displayName;
                Type outputPortType = edge.outputPort.portData.DisplayType;
                string inputNodeName = edge.inputNode.GetCustomName();
                string inputPortName = edge.inputPort.portData.displayName;
                return $"[Node: {outputNodeName}, OutputPort: {outputPortName}] emits {outputPortType} which is not compatible with [Node: {inputNodeName}, InputPort: {inputPortName}] with type of {expectedElementType}. If using a fixed size array such as T[] then this may cause issues. Skipping.";
            }

            try
            {
                // We know that this port is the input
                MemberInfo inputField = fieldInfo;
                Type inType = inputField.GetUnderlyingType();

                if (!IsInput)
                {
                    Debug.LogError(IsNotInputErrorMessage());
                    return null;
                }
                if (!inType.IsCollection())
                {
                    Debug.LogError(IsNotCollectionTypeErrorMessage(inType));
                    return null;
                }

                EdgeProcessOrderCallback edgeProcessOrderCallback = EdgeProcessOrderCallbackByKey[portData.edgeProcessOrder];
                var outputValues = Activator.CreateInstance(inType, new object[] { edges.Count });
                Type elementType = inType.GetCollectionElementType() ?? typeof(object);
                CopyEdgeBufferToIList(edgeProcessOrderCallback(edges), elementType, outputValues as IList);

                Expression inputParamField = Expression.PropertyOrField(Expression.Constant(owner), inputField.Name);
                Expression outputParamField = Expression.Constant(outputValues);

                Type outType = outputValues.GetType();

                // If there is a user defined conversion function, then we call it
                if (TypeAdapter.AreAssignable(outType, inType))
                {
                    // We add a cast in case there we're calling the conversion method with a base class parameter (like object)
                    var convertedParam = Expression.Convert(outputParamField, outType);
                    outputParamField = Expression.Call(TypeAdapter.GetConversionMethod(outType, inType), convertedParam);
                    // In case there is a custom port behavior in the output, then we need to re-cast to the base type because
                    // the conversion method return type is not always assignable directly:
                    outputParamField = Expression.Convert(outputParamField, inType);
                }
                else // otherwise we cast
                    outputParamField = Expression.Convert(outputParamField, inType);

                BinaryExpression assign = Expression.Assign(inputParamField, outputParamField);
                return Expression.Lambda<PushDataDelegate>(assign).Compile();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }


            static void CopyEdgeBufferToIList(IList<SerializableEdge> edges, Type expectedElementType, in IList toIList)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    if (!expectedElementType.IsReallyAssignableFrom(edges[i].passThroughBuffer.GetType()))
                    {
                        Debug.LogError(UnexpectedElementTypeErrorMessage(edges[i], expectedElementType));
                        continue;
                    }

                    if (toIList.IsFixedSize) toIList[i] = edges[i].passThroughBuffer;
                    else toIList.Add(edges[i].passThroughBuffer);
                }
            }
        }

        /// <summary>
        /// Disconnect an Edge from this port
        /// </summary>
        /// <param name="edge"></param>
        public void Remove(SerializableEdge edge)
        {
            if (!edges.Contains(edge))
                return;

            pushDataDelegates.Remove(edge);
            edgeWithRemoteCustomIO.Remove(edge);
            edges.Remove(edge);
        }

        /// <summary>
        /// Get all the edges connected to this port
        /// </summary>
        /// <returns></returns>
        public List<SerializableEdge> GetEdges() => edges;

        /// <summary>
        /// Push the value of the port through the edges
        /// This method can only be called on output ports
        /// </summary>
        public void PushData()
        {
            if (customPortIOMethod != null)
            {
                customPortIOMethod(owner, edges, this);
                return;
            }

            foreach (var pushDataDelegate in pushDataDelegates)
                pushDataDelegate.Value();


            object ourValue = fieldInfo.GetValue(fieldOwner);
            foreach (var edge in edgeWithArrayInput)
                edge.passThroughBuffer = ourValue;

            if (edgeWithRemoteCustomIO.Count == 0)
                return;

            //if there are custom IO implementation on the other ports, they'll need our value in the passThrough buffer
            foreach (var edge in edgeWithRemoteCustomIO)
                edge.passThroughBuffer = ourValue;
        }

        /// <summary>
        /// Reset the value of the field to default if possible
        /// </summary>
        public void ResetToDefault()
        {
            // Clear lists, set classes to null and struct to default value.
            if (typeof(IList).IsAssignableFrom(fieldInfo.GetUnderlyingType()))
                (fieldInfo.GetValue(fieldOwner) as IList)?.Clear();
            else if (fieldInfo.GetUnderlyingType().GetTypeInfo().IsClass)
                fieldInfo.SetValue(fieldOwner, null);
            else
            {
                try
                {
                    fieldInfo.SetValue(fieldOwner, Activator.CreateInstance(fieldInfo.GetUnderlyingType()));
                }
                catch { } // Catch types that don't have any constructors
            }
        }

        /// <summary>
        /// Pull values from the edge (in case of a custom conversion method)
        /// This method can only be called on input ports
        /// </summary>
        public void PullData()
        {
            if (customPortIOMethod != null)
            {
                customPortIOMethod(owner, edges, this);
                return;
            }

            // check if this port have connection to ports that have custom output functions
            if (!IsMultiEdgeInput && edgeWithRemoteCustomIO.Count == 0)
                return;

            // Only one input connection is handled by this code, if you want to
            // take multiple inputs, you must create a custom input function see CustomPortsNode.cs
            if ((!IsMultiEdgeInput || !fieldInfo.GetUnderlyingType().IsCollection()) && edges.Count > 0)
            {
                var passThroughObject = edges.First().passThroughBuffer;

                // We do an extra conversion step in case the buffer output is not compatible with the input port
                if (passThroughObject != null)
                    if (TypeAdapter.AreAssignable(fieldInfo.GetUnderlyingType(), passThroughObject.GetType()))
                        passThroughObject = TypeAdapter.Convert(passThroughObject, fieldInfo.GetUnderlyingType());

                fieldInfo.SetValue(fieldOwner, passThroughObject);
            }
            else if (IsMultiEdgeInput)
            {
                CreatePushDataDelegateForMultiEdgeInput(edges)();
            }
        }
    }

    /// <summary>
    /// Container of ports and the edges connected to these ports
    /// </summary>
    public abstract class NodePortContainer : List<NodePort>
    {
        protected BaseNode node;

        public NodePortContainer(BaseNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Remove an edge that is connected to one of the node in the container
        /// </summary>
        /// <param name="edge"></param>
        public void Remove(SerializableEdge edge)
        {
            ForEach(p => p.Remove(edge));
        }

        /// <summary>
        /// Add an edge that is connected to one of the node in the container
        /// </summary>
        /// <param name="edge"></param>
        public void Add(SerializableEdge edge)
        {
            string portFieldName = (edge.inputNode == node) ? edge.inputFieldName : edge.outputFieldName;
            string portIdentifier = (edge.inputNode == node) ? edge.inputPortIdentifier : edge.outputPortIdentifier;

            // Force empty string to null since portIdentifier is a serialized value
            if (String.IsNullOrEmpty(portIdentifier))
                portIdentifier = null;

            var port = this.FirstOrDefault(p =>
            {
                return p.fieldName == portFieldName && p.portData.Identifier == portIdentifier;
            });

            if (port == null)
            {
                Debug.LogError("The edge can't be properly connected because it's ports can't be found");
                return;
            }

            port.Add(edge);
        }
    }

    /// <inheritdoc/>
    public class NodeInputPortContainer : NodePortContainer
    {
        public NodeInputPortContainer(BaseNode node) : base(node) { }

        public void PullDatas()
        {
            ForEach(p => p.PullData());
        }
    }

    /// <inheritdoc/>
    public class NodeOutputPortContainer : NodePortContainer
    {
        public NodeOutputPortContainer(BaseNode node) : base(node) { }

        public void PushDatas()
        {
            ForEach(p => p.PushData());
        }
    }
}