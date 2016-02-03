using System;
using System.Numerics;
using System.Reactive.Linq;
using SkiManager.Engine.Interfaces;

using Windows.Foundation;
using Windows.UI;

namespace SkiManager.Engine.Behaviors
{
    public sealed class SimpleGeometryRendererBehavior : ReactiveBehavior, IRenderer
    {
        public SimpleGeometry Geometry { get; set; }

        public Size Size { get; set; }

        public Color Color { get; set; } = Colors.Black;

        public bool IsVisible { get; set; } = true;

        public bool IsEffectivelyVisible => IsVisible && Entity.Parent == null;

        public RenderLayer RenderLayer { get; set; } = RenderLayer.Default;

        public bool DrawCenter { get; set; } = true;

        public bool FillGeometry { get; set; }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.WhereLayerIsCorrect(this).Where(CanRender).Subscribe(OnRender));
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            var absolutePosition = Entity.GetBehavior<TransformBehavior>().Position.XZ();
            var halfSize = new Vector2((float)(Size.Width / 2), (float)(Size.Height / 2));
            switch (Geometry)
            {
                case SimpleGeometry.Circle:
                    if (FillGeometry)
                    {
                        args.DrawingSession.FillCircle(absolutePosition, (float)(Size.Width / 2), Color);
                    }
                    else
                    {
                        args.DrawingSession.DrawCircle(absolutePosition, (float)(Size.Width / 2), Color);
                    }
                    break;
                case SimpleGeometry.Square:
                    var pos = absolutePosition - halfSize;
                    if (FillGeometry)
                    {
                        args.DrawingSession.FillRectangle(new Rect(pos.X, pos.Y, Size.Width, Size.Height), Color);
                    }
                    else
                    {
                        args.DrawingSession.DrawRectangle(new Rect(pos.X, pos.Y, Size.Width, Size.Height), Color);
                    }
                    break;
            }
            if (DrawCenter)
            {
                var color = Color;
                if (FillGeometry)
                {
                    color = Colors.Black;
                }
                args.DrawingSession.DrawCircle(absolutePosition, 1.0f, color);
            }
        }

        private bool CanRender(EngineDrawEventArgs args) => IsEffectivelyVisible;
    }

    public enum SimpleGeometry
    {
        Circle,
        Square
    }
}
