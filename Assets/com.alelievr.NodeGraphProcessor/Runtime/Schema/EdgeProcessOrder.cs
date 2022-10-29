using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor.EdgeProcessing
{
    public static class EdgeProcessOrder
    {
        public const string DefaultEdgeProcessOrder = FIFO;
        public const string FIFO = nameof(FIFO);
        public const string TOP_TO_BOTTOM = nameof(TOP_TO_BOTTOM);
        public const string BOTTOM_TO_TOP = nameof(BOTTOM_TO_TOP);

        [EdgeOrderer(FIFO)]
        public static IList<SerializableEdge> OrderByFIFO(IList<SerializableEdge> edges)
        {
            return edges; // Default Behaviour. NOTE: In the future we could add a timestamp so we know when something was first connected for sure
        }

        [EdgeOrderer(TOP_TO_BOTTOM)]
        public static IList<SerializableEdge> OrderByTopToBottom(IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => x.outputNode.position.y).ToList();
        }

        [EdgeOrderer(BOTTOM_TO_TOP)]
        public static IList<SerializableEdge> OrderByBottomToTop(IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => -x.outputNode.position.y).ToList();
        }
    }
}