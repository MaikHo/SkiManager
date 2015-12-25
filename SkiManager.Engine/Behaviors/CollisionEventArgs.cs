using System.Numerics;

namespace SkiManager.Engine.Behaviors
{
    public sealed class CollisionEventArgs : EngineEventArgs
    {
        public ColliderType ColliderType { get; }

        public Vector2 CollisionPoint { get; }

        public CollisionEventArgs(Engine engine, ColliderType colliderType, Vector2 collisionPoint) : base(engine)
        {
            ColliderType = colliderType;
            CollisionPoint = collisionPoint;
        }
    }
}
