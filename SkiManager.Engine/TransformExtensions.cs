using System.Numerics;

using SkiManager.Engine.Behaviors;

namespace SkiManager.Engine
{
    public static class TransformExtensions
    {
        public static Vector2 GetRelativePosition(this TransformBehavior transform)
        {
            var thisPos = transform.Position;
            var parentPos = transform.Entity?.Parent?.GetBehavior<TransformBehavior>()?.Position ?? Vector2.Zero;
            var result = thisPos - parentPos;
            return result;
        }
    }
}
