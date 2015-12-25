using System;

using Windows.UI;

using SkiManager.Engine.Behaviors;

namespace SkiManager.Engine.Features
{
    public sealed class DebugRenderEntityEngineFeature : EngineFeature
    {
        private IDisposable _subscription;

        public Entity Entity { get; set; }

        public DebugRenderEntityEngineFeature(Entity entity)
        {
            Entity = entity;
        }

        protected override void Attach()
        {
            _subscription = Engine.Events.Draw.Subscribe(OnRender);
        }

        protected override void Detach()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            var pos = Entity.GetBehavior<TransformBehavior>().GetAbsolutePosition();
            args.Arguments.DrawingSession.DrawRectangle(pos.X, pos.Y, 1, 1, Colors.Blue);
            args.Arguments.DrawingSession.DrawRectangle(pos.X - 25, pos.Y - 25, 50, 50, Colors.Blue);
        }
    }
}
