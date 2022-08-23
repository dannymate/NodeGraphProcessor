using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    public class BaseGraph : GraphBase
    {
        [SerializeReference]
        public List<BaseNode> nodes = new();
        public override List<BaseNode> Nodes => nodes;
    }
}