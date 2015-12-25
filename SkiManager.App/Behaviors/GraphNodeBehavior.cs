using System.Collections.Generic;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public class GraphNodeBehavior : ReactiveBehavior, IGraphNode
    {
        private List<Entity> _adjacentEdges = new List<Entity>();
        public IList<Entity> AdjacentEdges => _adjacentEdges;
    }
}
