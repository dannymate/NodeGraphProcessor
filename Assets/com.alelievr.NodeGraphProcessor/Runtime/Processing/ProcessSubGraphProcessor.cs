using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphProcessor
{

    /// <summary>
    /// Graph processor
    /// </summary>
    public class ProcessSubGraphProcessor : BaseSubGraphProcessor
    {
        List<BaseNode> processList;

        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public ProcessSubGraphProcessor(SubGraph graph) : base(graph)
        {
            // Later on if there's interference issues when a SubGraph is run before returning its results
            // this.graph = ScriptableObject.Instantiate(graph);
        }

        public override void UpdateComputeOrder()
        {
            processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
        }

        /// <summary>
        /// Process all the nodes following the compute order.
        /// </summary>
        public override void Run(Dictionary<PortData, object> ingress)
        {
            int count = processList.Count;

            this.graph.IngressNode?.PullIngress(ingress);

            for (int i = 0; i < count; i++)
                processList[i].OnProcess();
        }
    }
}
