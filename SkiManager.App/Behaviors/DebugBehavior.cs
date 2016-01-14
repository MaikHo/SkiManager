using System;
using System.Numerics;
using System.Reactive.Linq;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Features;
using SkiManager.Engine.Interfaces;

namespace SkiManager.App.Behaviors
{
    public sealed class DebugBehavior : ReactiveBehavior
    {
        private bool _isMouseOver = false;
        private Color _mouseOverIndicatorColor = Colors.Magenta;

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.Where(a => a.RenderLayer == RenderLayers.TopMost).Subscribe(OnDraw));
            if (Entity.Implements<IReadOnlyTransform>())
            {
                args.TrackSubscription(MouseInteractionFeature.MouseEnterFor(Entity).Subscribe(OnMouseEnter));
                args.TrackSubscription(MouseInteractionFeature.MouseMoveOverFor(Entity).Subscribe(OnMouseMove));
                args.TrackSubscription(MouseInteractionFeature.MouseLeaveFor(Entity).Subscribe(OnMouseLeave));
            }
        }

        private void OnMouseEnter(MouseEnterEngineEventArgs args)
        {
            _isMouseOver = true;
        }

        private void OnMouseMove(MouseMoveOverEngineEventArgs args)
        {
            _mouseOverIndicatorColor = Colors.Lime;
        }

        private void OnMouseLeave(MouseLeaveEngineEventArgs args)
        {
            _isMouseOver = false;
            _mouseOverIndicatorColor = Colors.Magenta;
        }

        private void OnDraw(EngineDrawEventArgs args)
        {
            if (Entity == null)
            {
                return;
            }

            var entityLabel = Entity.Name + "[" + Entity.Id + "], Loc: " + (Entity.Parent?.Name ?? "<none>");
            var position = Entity.GetBehavior<TransformBehavior>()?.Position.XZ();
            if (position == null)
            {
                return;
            }

            using (var format = new CanvasTextFormat { FontSize = 11 })
            {
                args.DrawingSession.DrawText(
                    entityLabel,
                    position.Value + new Vector2(0, -20),
                    Colors.DarkGray,
                    format);
            }

            if (_isMouseOver)
            {
                args.DrawingSession.DrawCircle(position.Value, 6.0f, _mouseOverIndicatorColor, 2.5f);
            }
        }
    }
}
