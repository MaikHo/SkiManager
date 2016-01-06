using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Numerics;
using System.Reactive.Linq;
using Windows.UI;

using SkiManager.Engine.Interfaces;
using SkiManager.Engine.Sprites;

namespace SkiManager.Engine.Behaviors
{
    public sealed class LineRendererBehavior : ReactiveBehavior, IRenderer
    {
        private SpriteReference _sprite;
        private ICanvasBrush _spriteBrush;

        public Func<Entity, Vector3> StartPointSelector { get; set; }

        public Func<Entity, Vector3> EndPointSelector { get; set; }

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

        public LineRendererBehavior() : this(null, null)
        { }

        public LineRendererBehavior(Func<Entity, Vector3> startPointSelector, Func<Entity, Vector3> endPointSelector)
        {
            StartPointSelector = startPointSelector;
            EndPointSelector = endPointSelector;
        }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Draw.Where(_ => IsVisible).Subscribe(OnRender));
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            if (StartPointSelector == null || EndPointSelector == null)
            {
                return;
            }

            var coordinateSystem = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var startPoint = coordinateSystem.TransformToDips(StartPointSelector(Entity));
            var endPoint = coordinateSystem.TransformToDips(EndPointSelector(Entity));

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
