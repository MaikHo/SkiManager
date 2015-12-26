using System;
using System.Numerics;
using Windows.Foundation;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App
{
    [RequiresBehavior(typeof(TransformBehavior))]
    [RequiresBehavior(typeof(ShapeColliderBehavior))]
    public class TestBehavior : ReactiveBehavior
    {
        private static readonly Random _random = new Random();

        private Size _canvasSize;
        private IDisposable _updateSubscription;
        private IDisposable _collisionSubscription;

        protected override void Loaded()
        {
            _updateSubscription = Update.Subscribe(OnUpdate);
            _collisionSubscription = Entity.GetBehavior<ShapeColliderBehavior>().Collision.Subscribe(_ => Move());
        }

        protected override void Unloading()
        {
            _updateSubscription?.Dispose();
            _collisionSubscription?.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            _canvasSize = args.Sender.Size;
        }

        private void Move()
        {
            Entity.GetBehavior<TransformBehavior>().Position = new Vector2((float)(_random.NextDouble() * _canvasSize.Width), (float)(_random.NextDouble() * _canvasSize.Height));
        }
    }
}
