using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Reactive.Linq;
using Windows.UI;

using SkiManager.Engine.Interfaces;
using SkiManager.Engine.Sprites;

namespace SkiManager.Engine.Behaviors
{
    [RequiresImplementation(typeof(ILineTransform))]
    public sealed class LineRendererBehavior : ReactiveBehavior, IRenderer
    {
        private SpriteReference _sprite;
        private ICanvasBrush _spriteBrush;

        /// <summary>
        /// The line color. This value is ignored when a sprite is specified.
        /// </summary>
        public Color Color { get; set; } = Colors.Black;

        /// <summary>
        /// The sprite that is repeated along the line.
        /// </summary>
        public SpriteReference Sprite
        {
            get { return _sprite; }
            set
            {
                if (!Equals(_sprite, value))
                {
                    _sprite = value;
                    _spriteBrush?.Dispose();
                    _spriteBrush = null;
                }
            }
        }

        public bool IsVisible { get; set; } = true;
        public RenderLayer RenderLayer { get; set; } = RenderLayer.Default;

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.WhereShouldRender(this).Subscribe(OnRender));
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            var coordinateSystem = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var lineTransform = Entity.GetImplementation<ILineTransform>();

            if (coordinateSystem == null || lineTransform == null)
            {
                return;
            }

            var startPoint = coordinateSystem.TransformToDips(lineTransform.Point1);
            var endPoint = coordinateSystem.TransformToDips(lineTransform.Point2);

            var sprite = Sprite.Resolve(Entity);

            if (sprite == null)
            {
                // Use Color
                // TODO: Stroke width?
                args.DrawingSession.DrawLine(startPoint, endPoint, Color);
            }
            else
            {
                // Use Sprite
                // TODO: Respect sprite size and adjust stroke width accordingly
                // TODO: Rotate sprite depending on the angle of the line
                if (_spriteBrush == null)
                {
                    var brush = new CanvasImageBrush(args.Sender, sprite.Image);
                    brush.ExtendX = Microsoft.Graphics.Canvas.CanvasEdgeBehavior.Wrap;
                    brush.ExtendY = Microsoft.Graphics.Canvas.CanvasEdgeBehavior.Wrap;
                    _spriteBrush = brush;
                }

                args.DrawingSession.DrawLine(startPoint, endPoint, _spriteBrush);
            }
        }
    }
}
