using System;
using System.Numerics;

using Windows.UI;

namespace SkiManager.Engine.Features
{
    public class TrackMousePositionEngineFeature : EngineFeature
    {
        private readonly bool _highlightMousePosition;
        private IDisposable _subscription;
        private IDisposable _debugSubscription;

        public Vector2 LastMouseScreenPosition { get; private set; }

        public TrackMousePositionEngineFeature(bool highlightMousePosition = false)
        {
            _highlightMousePosition = highlightMousePosition;
        }

        protected override void Attach()
        {
            _subscription = Engine.Events.PointerMoved.Subscribe(OnPointerMoved);
            if (_highlightMousePosition)
            {
                _debugSubscription = Engine.Events.Draw.Subscribe(Draw);
            }
        }

        protected override void Detach()
        {
            _subscription?.Dispose();
            _subscription = null;
            _debugSubscription?.Dispose();
            _debugSubscription = null;
        }

        private void OnPointerMoved(EnginePointerMovedEventArgs args)
        {
            LastMouseScreenPosition = args.Arguments.GetCurrentPoint(args.Sender).RawPosition.ToVector2();
        }

        protected virtual void Draw(EngineDrawEventArgs e)
        {
            e.DrawingSession.DrawCircle(LastMouseScreenPosition, 10, Colors.Red);
        }
    }
}
