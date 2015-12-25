using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public class GraphEdgeBehavior : ReactiveBehavior, IGraphEdge
    {
        public Entity Start { get; set; }

        public Entity End { get; set; }
    }
}
