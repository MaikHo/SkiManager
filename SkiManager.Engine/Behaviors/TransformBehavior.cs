using System.Numerics;

using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Behaviors
{
    public sealed class TransformBehavior : ReactiveBehavior, ITransform
    {
        public Vector2 Position { get; set; }
    }
}
