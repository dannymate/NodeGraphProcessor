using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class GraphPresenter
    {
        private BaseGraphView _view;
        private BaseGraph _model;

#if UNITY_2020_1_OR_NEWER
        /// <summary>
        /// List of all sticky note presenters in the graph
        /// </summary>
        /// <typeparam name="StickyNotePresenter"></typeparam>
        /// <returns></returns>
        private List<StickyNotePresenter> _stickyNotes = new();
#endif
#if UNITY_2020_1_OR_NEWER

        public void AddStickyNote(StickyNote note)
        {
            _model.AddStickyNote(note);
        }

        public void RemoveStickyNote(StickyNote note)
        {
            _model.RemoveStickyNote(note);
        }

        public event Action<StickyNote> StickyNoteDeletionRequest;

        private void OnStickyNoteRemoveRequest(StickyNotePresenter presenter, StickyNoteView view, StickyNote model)
        {
            _model.RemoveStickyNote(model, true);
            _view.RemoveElement(view);
            _stickyNotes.Remove(presenter);
        }

        private void OnStickyNoteAdded(StickyNote note)
        {
            BuildStickyNote(note);
        }

        private void BuildStickyNote(StickyNote note)
        {
            StickyNoteView noteView = StickyNoteView.Instantiate(note.title, note.contents, note.position);
            StickyNotePresenter notePresenter = new(this, note, noteView);

            notePresenter.RemovedRequested += OnStickyNoteRemoveRequest;

            _view.AddElement(noteView);
            _stickyNotes.Add(notePresenter);

        }

        private void OnStickyNoteRemoved(StickyNote note)
        {
            StickyNoteDeletionRequest?.Invoke(note);
        }

        public void RemoveStickyNotes()
        {
            foreach (var stickyNote in _stickyNotes)
                stickyNote.Remove();
        }
#endif
        public GraphPresenter()
        {
            _model.StickyNoteAdded += OnStickyNoteAdded;
            _model.StickyNoteRemoved += OnStickyNoteRemoved;
            // _view.NodeDuplicated += OnNodeDuplicated;
            // _view.serializeGraphElements = SerializeGraphElementsCallback;
            // _view.unserializeAndPaste = DeserializeAndPasteCallback;
            // _view.canPasteSerializedData = CanPasteSerializedDataCallback;
            // _view.graphViewChanged = GraphViewChangedCallback;
            // _view.viewTransformChanged = ViewTransformChangedCallback;
            // _view.elementResized = ElementResizedCallback;

            InitializeStickyNotes();
        }

        void InitializeStickyNotes()
        {
#if UNITY_2020_1_OR_NEWER
            foreach (var note in _model.stickyNotes)
                BuildStickyNote(note);
#endif
        }

        //         void ViewTransformChangedCallback(GraphView view)
        //         {
        //             if (_model != null)
        //             {
        //                 _model.position = _view.viewTransform.position;
        //                 _model.scale = _view.viewTransform.scale;
        //             }
        //         }

        //         void ElementResizedCallback(VisualElement elem)
        //         {
        //             if (elem is GroupView groupView)
        //                 groupView.group.size = groupView.GetPosition().size;
        //         }

        //         public void RegisterCompleteObjectUndo(string name)
        //         {
        //             Undo.RegisterCompleteObjectUndo(_model, name);
        //         }

        //         GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        //         {
        //             if (changes.elementsToRemove != null)
        //             {
        //                 RegisterCompleteObjectUndo("Remove Graph Elements");

        //                 // Destroy priority of objects
        //                 // We need nodes to be destroyed first because we can have a destroy operation that uses node connections
        //                 changes.elementsToRemove.Sort((e1, e2) =>
        //                 {
        //                     int GetPriority(GraphElement e)
        //                     {
        //                         if (e is BaseNodeView)
        //                             return 0;
        //                         else
        //                             return 1;
        //                     }
        //                     return GetPriority(e1).CompareTo(GetPriority(e2));
        //                 });

        //                 //Handle ourselves the edge and node remove
        //                 changes.elementsToRemove.RemoveAll(e =>
        //                 {

        //                     switch (e)
        //                     {
        //                         case EdgeView edge:
        //                             Disconnect(edge);
        //                             return true;
        //                         case BaseNodeView nodeView:
        //                             // For vertical nodes, we need to delete them ourselves as it's not handled by GraphView
        //                             foreach (var pv in nodeView.inputPortViews.Concat(nodeView.outputPortViews))
        //                                 if (pv.orientation == Orientation.Vertical)
        //                                     foreach (var edge in pv.GetEdges().ToList())
        //                                         Disconnect(edge);

        //                             nodeInspector.NodeViewRemoved(nodeView);
        //                             ExceptionToLog.Call(() => nodeView.OnRemoved());
        //                             RemoveNode(nodeView.nodeTarget);
        //                             UpdateSerializedProperties();
        //                             RemoveElement(nodeView);
        //                             if (Selection.activeObject == nodeInspector)
        //                                 UpdateNodeInspectorSelection();

        //                             SyncSerializedPropertyPathes();
        //                             return true;
        //                         case GroupView group:
        //                             _model.RemoveGroup(group.group);
        //                             UpdateSerializedProperties();
        //                             RemoveElement(group);
        //                             return true;
        //                         case ExposedParameterFieldView blackboardField:
        //                             _model.RemoveExposedParameter(blackboardField.parameter);
        //                             UpdateSerializedProperties();
        //                             return true;
        //                         case BaseStackNodeView stackNodeView:
        //                             _model.RemoveStackNode(stackNodeView.stackNode);
        //                             UpdateSerializedProperties();
        //                             RemoveElement(stackNodeView);
        //                             return true;
        // #if UNITY_2020_1_OR_NEWER
        //                         case StickyNoteView stickyNoteView:
        //                             _model.RemoveStickyNote(stickyNoteView.note);
        //                             UpdateSerializedProperties();
        //                             RemoveElement(stickyNoteView);
        //                             return true;
        // #endif
        //                     }

        //                     return false;
        //                 });
        //             }

        //             return changes;
        //         }

        //         void DeserializeAndPasteCallback(string operationName, string serializedData)
        //         {
        //             var data = JsonUtility.FromJson<CopyPasteHelper>(serializedData);

        //             RegisterCompleteObjectUndo(operationName);

        //             Dictionary<PropertyName, BaseNode> copiedNodesMap = new();

        //             var unserializedGroups = data.copiedGroups.Select(g => JsonSerializer.Deserialize<Group>(g)).ToList();

        //             foreach (var serializedNode in data.copiedNodes)
        //             {
        //                 var node = JsonSerializer.DeserializeNode(serializedNode);

        //                 if (node == null)
        //                     continue;

        //                 PropertyName sourceGUID = node.GUID;
        //                 _model.nodesPerGUID.TryGetValue(sourceGUID, out var sourceNode);
        //                 //Call OnNodeCreated on the new fresh copied node
        //                 node.createdFromDuplication = true;
        //                 node.createdWithinGroup = unserializedGroups.Any(g => g.innerNodeGUIDs.Contains(sourceGUID));
        //                 node.OnNodeCreated();
        //                 //And move a bit the new node
        //                 node.initialPosition = new Rect(node.position.position + new Vector2(20, 20), node.initialPosition.size);

        //                 var newNodeView = AddNode(node);

        //                 // If the nodes were copied from another graph, then the source is null FIRE EVENT
        //                 // if (sourceNode != null)
        //                 //     NodeDuplicated?.Invoke(sourceNode, node);

        //                 copiedNodesMap[sourceGUID] = node;

        //                 //Select the new node
        //                 _view.AddToSelection(nodeViewsPerNode[node]);
        //             }

        //             foreach (var group in unserializedGroups)
        //             {
        //                 //Same than for node
        //                 group.OnCreated();

        //                 // try to centre the created node in the screen
        //                 group.position.position += new Vector2(20, 20);

        //                 var oldGUIDList = group.innerNodeGUIDs.ToList();
        //                 group.innerNodeGUIDs.Clear();
        //                 foreach (var guid in oldGUIDList)
        //                 {
        //                     _model.nodesPerGUID.TryGetValue(guid, out var node);

        //                     // In case group was copied from another graph
        //                     if (node == null)
        //                     {
        //                         copiedNodesMap.TryGetValue(guid, out node);
        //                         group.innerNodeGUIDs.Add(node.GUID);
        //                     }
        //                     else
        //                     {
        //                         group.innerNodeGUIDs.Add(copiedNodesMap[guid].GUID);
        //                     }
        //                 }

        //                 AddGroup(group);
        //             }

        //             foreach (var serializedEdge in data.copiedEdges)
        //             {
        //                 var edge = JsonSerializer.Deserialize<SerializableEdge>(serializedEdge);

        //                 edge.Deserialize();

        //                 // Find port of new nodes:
        //                 copiedNodesMap.TryGetValue(edge.inputNode.GUID, out var oldInputNode);
        //                 copiedNodesMap.TryGetValue(edge.outputNode.GUID, out var oldOutputNode);

        //                 // We avoid to break the graph by replacing unique connections:
        //                 if (oldInputNode == null && !edge.inputPort.portData.acceptMultipleEdges || !edge.outputPort.portData.acceptMultipleEdges)
        //                     continue;

        //                 oldInputNode = oldInputNode ?? edge.inputNode;
        //                 oldOutputNode = oldOutputNode ?? edge.outputNode;

        //                 var inputPort = oldInputNode.GetPort(edge.inputPort.fieldName, edge.inputPortIdentifier);
        //                 var outputPort = oldOutputNode.GetPort(edge.outputPort.fieldName, edge.outputPortIdentifier);

        //                 var newEdge = SerializableEdge.CreateNewEdge(graph, inputPort, outputPort);

        //                 if (nodeViewsPerNode.ContainsKey(oldInputNode) && nodeViewsPerNode.ContainsKey(oldOutputNode))
        //                 {
        //                     var edgeView = CreateEdgeView();
        //                     edgeView.userData = newEdge;
        //                     edgeView.input = nodeViewsPerNode[oldInputNode].GetPortViewFromFieldName(newEdge.inputFieldName, newEdge.inputPortIdentifier);
        //                     edgeView.output = nodeViewsPerNode[oldOutputNode].GetPortViewFromFieldName(newEdge.outputFieldName, newEdge.outputPortIdentifier);

        //                     Connect(edgeView);
        //                 }
        //             }
        //         }

        //         public virtual EdgeView CreateEdgeView()
        //         {
        //             return new EdgeView();
        //         }

        //         bool CanPasteSerializedDataCallback(string serializedData)
        //         {
        //             try
        //             {
        //                 return JsonUtility.FromJson(serializedData, typeof(CopyPasteHelper)) != null;
        //             }
        //             catch
        //             {
        //                 return false;
        //             }
        //         }

        //         string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        //         {
        //             var data = new CopyPasteHelper();

        //             foreach (BaseNodeView nodeView in elements.Where(e => e is BaseNodeView))
        //             {
        //                 data.copiedNodes.Add(JsonSerializer.SerializeNode(nodeView.nodeTarget));
        //                 foreach (var port in nodeView.nodeTarget.GetAllPorts())
        //                 {
        //                     if (port.portData.vertical)
        //                     {
        //                         foreach (var edge in port.GetEdges())
        //                             data.copiedEdges.Add(JsonSerializer.Serialize(edge));
        //                     }
        //                 }
        //             }

        //             foreach (GroupView groupView in elements.Where(e => e is GroupView))
        //                 data.copiedGroups.Add(JsonSerializer.Serialize(groupView.group));

        //             foreach (EdgeView edgeView in elements.Where(e => e is EdgeView))
        //                 data.copiedEdges.Add(JsonSerializer.Serialize(edgeView.serializedEdge));

        //             _view.ClearSelection();

        //             return JsonUtility.ToJson(data, true);
        //         }

        //         // public delegate void NodeDuplicatedDelegate(BaseNode duplicatedNode, BaseNode newNode);
        //         /// <summary>
        //         /// Triggered when a node is duplicated (crt-d) or copy-pasted (crtl-c/crtl-v)
        //         /// </summary>
        //         // public event NodeDuplicatedDelegate NodeDuplicated;


        //         private void OnNodeDuplicated(BaseNode duplicatedNode, BaseNode newNode)
        //         {

        //         }
    }
}