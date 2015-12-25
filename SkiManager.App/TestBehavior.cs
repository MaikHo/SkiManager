using System;
using System.Numerics;
using Windows.Foundation;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App
{
    public class TestBehavior : ReactiveBehavior
    {
        private static readonly Random _random = new Random();

        private Size _canvasSize;

        protected override void Loaded()
        {
            Update.Subscribe(OnUpdate);
            Entity.GetBehavior<ShapeColliderBehavior>().Collision.Subscribe(_ => Move());
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
