using System;
using System.Numerics;
using Windows.UI;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App.Behaviors
{
    public sealed class DebugBehavior : ReactiveBehavior
    {
        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.Subscribe(OnDraw));
        }

        private void OnDraw(EngineDrawEventArgs args)
        {
            var entityLabel = Entity?.Name + "[" + Entity?.Id + "], Loc: " + (Entity?.Parent?.Name ?? "<none>");
            var position = Entity.GetBehavior<TransformBehavior>()?.Position.XZ();
            if (position == null)
                return;

            args.DrawingSession.DrawText(
                entityLabel,
                position.Value + new Vector2(0, -20),
                Colors.DarkGray,
                new Microsoft.Graphics.Canvas.Text.CanvasTextFormat
                {
                    FontSize = 11
                });
        }
    }
}
