using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public abstract class GraphEdgeBehavior : ReactiveBehavior, IGraphEdge
    {
        public Entity Start { get; set; }

        public Entity End { get; set; }

        public float BaseSpeedModifier { get; set; }
    }
}
