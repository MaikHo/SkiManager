using System.Numerics;

using SkiManager.Engine.Behaviors;

namespace SkiManager.Engine
{
    public static class TransformExtensions
    {
        public static Vector2 GetAbsolutePosition(this TransformBehavior transform)
        {
            var thisPos = transform.Position;
            var parentPos = transform.Entity?.Parent?.GetBehavior<TransformBehavior>()?.GetAbsolutePosition() ?? Vector2.Zero;
            var result = thisPos + parentPos;
            return result;
        }
    }
}
