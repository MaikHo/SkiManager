using System;
using System.Numerics;
using Windows.UI;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Features;
using SkiManager.Engine.Interfaces;

namespace SkiManager.App.Behaviors
{
    public sealed class DebugBehavior : ReactiveBehavior
    {
        private bool _isMouseOver = false;

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.Subscribe(OnDraw));
            if (Entity.Implements<IReadOnlyTransform>())
            {
                args.TrackSubscription(MouseInteractionFeature.MouseEnterFor(Entity).Subscribe(OnMouseEntered));
                args.TrackSubscription(MouseInteractionFeature.MouseLeaveFor(Entity).Subscribe(OnMouseLeave));
            }
        }

        private void OnMouseEntered(MouseEnterEngineEventArgs args)
        {
            _isMouseOver = true;
        }

        private void OnMouseLeave(MouseLeaveEngineEventArgs args)
        {
            _isMouseOver = false;
        }

        private void OnDraw(EngineDrawEventArgs args)
        {
            if (Entity == null)
            {
                return;
            }

            var entityLabel = Entity.Name + "[" + Entity.Id + "], Loc: " + (Entity.Parent?.Name ?? "<none>") + ", MouseEntered:" + _isMouseOver;
            var position = Entity.GetBehavior<TransformBehavior>()?.Position.XZ();
            if (position == null)
            {
                return;
            }

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
