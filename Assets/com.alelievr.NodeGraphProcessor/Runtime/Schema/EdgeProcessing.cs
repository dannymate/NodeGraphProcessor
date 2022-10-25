
using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor
{
    public static class EdgeProcessing
    {

        public enum EdgeProcessOrder { FIFO, TOP_TO_BOTTOM }

        public static IList<SerializableEdge> Order(this IList<SerializableEdge> edges, EdgeProcessOrder processOrder)
        {
            switch (processOrder)
            {
                case EdgeProcessOrder.FIFO:
                    break; // Default
                case EdgeProcessOrder.TOP_TO_BOTTOM:
                    edges = edges.OrderBy(x => x.outputNode.position.y).ToList();
                    break;
            }
            return edges;
        }
    }
}