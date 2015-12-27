using System;
using System.Numerics;

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

        public bool DrawCenter { get; set; } = true;

        protected override void Loaded()
        {
            Draw.Subscribe(OnRender);
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            var absolutePosition = Entity.GetBehavior<TransformBehavior>().Position;
            var halfSize = new Vector2((float)(Size.Width / 2), (float)(Size.Height / 2));
            switch (Geometry)
            {
                case SimpleGeometry.Circle:
                    args.DrawingSession.DrawCircle(absolutePosition, (float)(Size.Width / 2), Color);
                    break;
                case SimpleGeometry.Square:
                    var pos = absolutePosition - halfSize;
                    args.DrawingSession.DrawRectangle(new Rect(pos.X, pos.Y, Size.Width, Size.Height), Color);
                    break;
            }
            if (DrawCenter)
            {
                args.DrawingSession.DrawCircle(absolutePosition, 1.0f, Color);
            }
        }
    }

    public enum SimpleGeometry
    {
        Circle,
        Square
    }
}
