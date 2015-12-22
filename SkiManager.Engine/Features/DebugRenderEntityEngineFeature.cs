using System;

using Windows.UI;

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
            args.Arguments.DrawingSession.DrawRectangle(Entity.Location.Position.X, Entity.Location.Position.Y, 1, 1, Colors.Blue);
            args.Arguments.DrawingSession.DrawRectangle(Entity.Location.Position.X - 25, Entity.Location.Position.Y - 25, 50, 50, Colors.Blue);
        }
    }
}
