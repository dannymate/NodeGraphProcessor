using System.Collections.Generic;
// using Unity.Entities;

namespace GraphProcessor
{
    public abstract class BaseSubGraphProcessor
    {
        protected SubGraph graph;

        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public BaseSubGraphProcessor(SubGraph graph)
        {
            this.graph = graph;

            UpdateComputeOrder();
        }

        public abstract void UpdateComputeOrder();

        /// <summary>
        /// Schedule the graph into the job system
        /// </summary>
        public abstract void Run(Dictionary<PortDataRef, object> ingress);
    }
}
