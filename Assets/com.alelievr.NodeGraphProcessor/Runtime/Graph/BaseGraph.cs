using System.Collections;
using System.Collections.Generic;

namespace GraphProcessor
{
    public class BaseGraph : GraphBase
    {
        public List<BaseNode> nodes = new();
        public override List<BaseNode> Nodes => nodes;
    }
}