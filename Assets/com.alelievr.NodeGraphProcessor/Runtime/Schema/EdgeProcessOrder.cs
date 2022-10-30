using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor.EdgeProcessing
{
    public static class EdgeProcessOrder
    {
        public const string DefaultEdgeProcessOrder = FIFO;
        public const string FIFO = nameof(FIFO);
        public const string LIFO = nameof(LIFO);
        public const string TOP_TO_BOTTOM = nameof(TOP_TO_BOTTOM);
        public const string BOTTOM_TO_TOP = nameof(BOTTOM_TO_TOP);
        public const string LEFT_TO_RIGHT = nameof(LEFT_TO_RIGHT);
        public const string RIGHT_TO_LEFT = nameof(RIGHT_TO_LEFT);


        [EdgeOrderer(FIFO)]
        public static IList<SerializableEdge> OrderByFIFO(this IList<SerializableEdge> edges)
        {
            return edges; // Default Behaviour. NOTE: In the future we could add a timestamp so we know when something was first connected for sure
        }

        [EdgeOrderer(LIFO)]
        public static IList<SerializableEdge> OrderByLIFO(this IList<SerializableEdge> edges)
        {
            return edges.Reverse().ToList(); // Default Behaviour. NOTE: In the future we could add a timestamp so we know when something was first connected for sure
        }

        [EdgeOrderer(TOP_TO_BOTTOM)]
        public static IList<SerializableEdge> OrderByTopToBottom(this IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => x.outputNode.position.y).ToList();
        }

        [EdgeOrderer(BOTTOM_TO_TOP)]
        public static IList<SerializableEdge> OrderByBottomToTop(this IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => -x.outputNode.position.y).ToList();
        }

        [EdgeOrderer(LEFT_TO_RIGHT)]
        public static IList<SerializableEdge> OrderByLeftToRight(this IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => x.outputNode.position.x).ToList();
        }

        [EdgeOrderer(RIGHT_TO_LEFT)]
        public static IList<SerializableEdge> OrderByRightToLeft(this IList<SerializableEdge> edges)
        {
            return edges.OrderBy(x => -x.outputNode.position.x).ToList();
        }
    }
}