using System;
using System.Numerics;

using Windows.UI;
using Windows.UI.Xaml;

namespace SkiManager.Engine.Features
{
    public sealed class TrackMousePositionEngineFeature : EngineFeature
    {
        private readonly bool _highlightMousePosition;
        private IDisposable _subscription;
        private IDisposable _debugSubscription;

        public Vector2 LastMouseScreenPos { get; private set; }

        public TrackMousePositionEngineFeature(bool highlightMousePosition = false)
        {
            _highlightMousePosition = highlightMousePosition;
        }

        protected override void Attach()
        {
            _subscription = Engine.Events.PointerMoved.Subscribe(OnPointerMoved);
            if (_highlightMousePosition)
            {
                _debugSubscription =
                    Engine.Events.Draw.Subscribe(
                        _ => { _.Arguments.DrawingSession.DrawCircle(LastMouseScreenPos, 10.0f, Colors.Red); });
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
            LastMouseScreenPos = args.Arguments.GetCurrentPoint(args.Sender as UIElement).RawPosition.ToVector2();
        }
    }
}
