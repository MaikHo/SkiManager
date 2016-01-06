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

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Update.Subscribe(OnUpdate));
            args.TrackSubscription(Entity.GetBehavior<ShapeColliderBehavior>().Collision.Subscribe(_ => Move()));
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            _canvasSize = args.Sender.Size;
        }

        private void Move()
        {
            Entity.GetBehavior<TransformBehavior>().Position = new Vector3((float)(_random.NextDouble() * _canvasSize.Width), 0, (float)(_random.NextDouble() * _canvasSize.Height));
        }
    }
}
