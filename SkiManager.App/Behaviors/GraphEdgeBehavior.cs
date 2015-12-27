using System.Numerics;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App.Behaviors
{
    public abstract class GraphEdgeBehavior : ReactiveBehavior, IGraphEdge
    {
        public Entity Start { get; set; }

        public Entity End { get; set; }

        public float Length => Vector2.Distance(Start.GetBehavior<TransformBehavior>().Position, End.GetBehavior<TransformBehavior>().Position);

        public bool IsBidirectional { get; set; }

        public float BaseSpeedModifier { get; set; }
    }
}
