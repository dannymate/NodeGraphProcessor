using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace GraphProcessor
{
    // TODO: replace this by the new UnityEditor.Searcher package
    class CreateNodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        BaseGraphView graphView;
        EditorWindow window;
        Texture2D icon;
        EdgeView edgeFilter;
        PortView inputPortView;
        PortView outputPortView;

        public void Initialize(BaseGraphView graphView, EditorWindow window, EdgeView edgeFilter = null)
        {
            this.graphView = graphView;
            this.window = window;
            this.edgeFilter = edgeFilter;
            this.inputPortView = edgeFilter?.input as PortView;
            this.outputPortView = edgeFilter?.output as PortView;

            // Transparent icon to trick search window into indenting items
            if (icon == null)
                icon = new Texture2D(1, 1);
            icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            icon.Apply();
        }

        void OnDestroy()
        {
            if (icon != null)
            {
                DestroyImmediate(icon);
                icon = null;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            if (edgeFilter == null)
                CreateStandardNodeMenu(tree);
            else
                CreateEdgeNodeMenu(tree);

            return tree;
        }

        void CreateStandardNodeMenu(List<SearchTreeEntry> tree)
        {
            // Sort menu by alphabetical order and submenus
            var nodeEntries = graphView.FilterCreateNodeMenuEntries()
                .Concat(graphView.FilterCreateCustomNodeMenuEntries())
                .OrderBy(k => k.Path);

            var titlePaths = new HashSet<string>();

            foreach (var nodeEntry in nodeEntries)
            {
                var nodePath = nodeEntry.Path;
                var nodeName = nodePath;
                var level = 0;
                var parts = nodePath.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[^1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level = i + 1;

                        // Add section title if the node is in subcategory
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = level
                            });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName, icon))
                {
                    level = level + 1,
                    userData = nodeEntry
                });
            }
        }

        void CreateEdgeNodeMenu(List<SearchTreeEntry> tree)
        {
            var cachedCustomMenuItemMethods = NodeProvider.CustomMenuItemMethods().Concat(NodeProvider.GetCustomClassMenuItemMethods());
            var entries = NodeProvider.GetEdgeCreationNodeMenuEntry((edgeFilter.input ?? edgeFilter.output) as PortView, graphView.graph);

            var titlePaths = new HashSet<string>();

            var customMenuEntries = NodeProvider.GetCustomNodeMenuEntries(graphView.graph, cachedCustomMenuItemMethods);
            var menuEntries = NodeProvider.GetNodeMenuEntries(graphView.graph).Concat(customMenuEntries);

            tree.Add(new SearchTreeEntry(new GUIContent($"Relay", icon))
            {
                level = 1,
                userData = new NodeProvider.PortDescription
                {
                    nodeType = typeof(RelayNode),
                    portType = typeof(System.Object),
                    isInput = inputPortView != null,
                    portFieldName = inputPortView != null ? nameof(RelayNode.output) : nameof(RelayNode.input),
                    portIdentifier = "0",
                    portDisplayName = inputPortView != null ? "Out" : "In",
                }
            });

            var sortedMenuItems = entries.Select(port => (port, menuEntries.FirstOrDefault(kp => kp.NodeType == port.nodeType).Path)).OrderBy(e => e.Path);

            // Sort menu by alphabetical order and submenus
            foreach (var menuEntry in sortedMenuItems)
            {
                string portFieldName = menuEntry.port.portFieldName;
                NodeProvider.PortDescription port = menuEntry.port;
                Type portNodeType = port.nodeType;
                Type portType = port.portType;
                var filteredCustomNodePaths = NodeProvider.GetFilteredCustomNodeMenuEntries((edgeFilter.input ?? edgeFilter.output).portType, port, cachedCustomMenuItemMethods);
                foreach (var node in menuEntries.Where(kp => kp.NodeType == portNodeType))
                {
                    var nodePath = node.Path;

                    // Ignore the node if it's not in the create menu
                    if (String.IsNullOrEmpty(nodePath))
                        continue;

                    // Ignore the node if it has filters and it doesn't meet the requirements, contains doesn't work so we compare paths instead.
                    if (customMenuEntries.Contains(node) && !filteredCustomNodePaths.Contains(node))
                        continue;

                    var nodeName = nodePath;
                    var level = 0;
                    var parts = nodePath.Split('/');

                    if (parts.Length > 1)
                    {
                        level++;
                        nodeName = parts[^1];
                        var fullTitleAsPath = "";

                        for (var i = 0; i < parts.Length - 1; i++)
                        {
                            var title = parts[i];
                            fullTitleAsPath += title;
                            level = i + 1;

                            // Add section title if the node is in subcategory
                            if (!titlePaths.Contains(fullTitleAsPath))
                            {
                                tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                                {
                                    level = level
                                });
                                titlePaths.Add(fullTitleAsPath);
                            }
                        }
                    }

                    tree.Add(new SearchTreeEntry(new GUIContent($"{nodeName}:  {port.portDisplayName}", icon))
                    {
                        level = level + 1,
                        userData = new Tuple<NodeProvider.PortDescription, NodeProvider.NodeMenuEntry>(port, node)
                    });
                }
            }
        }

        // Node creation when validate a choice
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            // window to graph position
            var windowRoot = window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            if (searchTreeEntry.userData is NodeProvider.NodeMenuEntry)
            {
                NodeProvider.NodeMenuEntry userData = searchTreeEntry.userData as NodeProvider.NodeMenuEntry;
                Type nodeType = userData.NodeType;
                NodeUtils.NodeCreationMethod method = userData.CreationMethod;
                object[] methodArgs = userData.CreationMethodArgs;

                graphView.RegisterCompleteObjectUndo("Added " + nodeType);
                graphView.AddNode(method.Invoke(nodeType, graphMousePosition, methodArgs));
            }
            else if (searchTreeEntry.userData is Tuple<NodeProvider.PortDescription, NodeProvider.NodeMenuEntry>)
            {
                Tuple<NodeProvider.PortDescription, NodeProvider.NodeMenuEntry> userData = searchTreeEntry.userData as Tuple<NodeProvider.PortDescription, NodeProvider.NodeMenuEntry>;
                Type nodeType = userData.Item1.nodeType;
                NodeUtils.NodeCreationMethod method = userData.Item2.CreationMethod;
                object[] methodArgs = userData.Item2.CreationMethodArgs;

                graphView.RegisterCompleteObjectUndo("Added " + nodeType);
                BaseNodeView view = graphView.AddNode(method.Invoke(nodeType, graphMousePosition, methodArgs));

                var targetPort = view.GetPortViewFromFieldName(userData.Item1.portFieldName, userData.Item1.portIdentifier);
                if (inputPortView == null)
                    graphView.Connect(targetPort, outputPortView);
                else
                    graphView.Connect(inputPortView, targetPort);
            }
            else
            {
                NodeProvider.PortDescription userData = (NodeProvider.PortDescription)searchTreeEntry.userData;

                graphView.RegisterCompleteObjectUndo("Added " + userData.nodeType);
                var nodeView = graphView.AddNode(BaseNode.CreateFromType(userData.nodeType, graphMousePosition));
                var targetPort = nodeView.GetPortViewFromFieldName(userData.portFieldName, userData.portIdentifier);

                if (inputPortView == null)
                    graphView.Connect(targetPort, outputPortView);
                else
                    graphView.Connect(inputPortView, targetPort);
            }

            return true;
        }
    }
}