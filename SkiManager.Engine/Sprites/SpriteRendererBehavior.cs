﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Interfaces;
using System;
using Windows.Foundation;

namespace SkiManager.Engine.Sprites
{
    [RequiresBehavior(typeof(TransformBehavior))]
    public class SpriteRenderer : ReactiveBehavior
    {
        private IDisposable _drawSubscription;
        private StraightenEffect _rotateEffect;
        private SpriteReference _oldSprite;

        public SpriteReference Sprite { get; set; }

        protected override void Loaded()
        {
            _drawSubscription = Draw.Subscribe(OnDraw);
        }

        protected override void Unloading()
        {
            _drawSubscription.Dispose();
        }

        private void OnDraw(EngineDrawEventArgs e)
        {
            if (Sprite != _oldSprite)
            {
                // Sprite has changed since last draw
                _rotateEffect?.Dispose();
                _rotateEffect = null;
            }

            if (Sprite == SpriteReference.Empty)
                return;

            var transform = Entity.GetBehavior<TransformBehavior>();
            var coords = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var sprite = Sprite.Resolve(Entity);

            if (transform == null)
                throw new InvalidOperationException($"No {nameof(TransformBehavior)} is attached");

            if (coords != null && sprite != null)
            {
                var worldPos = transform.Position;

                var worldRect = new Rect(
                    worldPos.X - transform.Scale.X * sprite.Size.X / 2,
                    worldPos.Y - transform.Scale.X * sprite.Size.Y / 2,
                    transform.Scale.X * sprite.Size.X,
                    transform.Scale.X * sprite.Size.Y);

                var dipsRect = coords.TransformToDips(worldRect);

                ICanvasImage renderImage;

                // If rotation is 0 we can draw the sprite directly.
                // Otherwise, use a StraightenEffect to rotate.
                if (Math.Abs(transform.Rotation) <= float.Epsilon)
                {
                    renderImage = sprite.Image;
                }
                else
                {
                    _rotateEffect = _rotateEffect ?? new StraightenEffect { Source = sprite.Image };
                    _rotateEffect.Angle = transform.RotationRadians;
                    renderImage = _rotateEffect;
                }

                e.DrawingSession.DrawImage(renderImage, dipsRect, renderImage.GetBounds(e.DrawingSession));
            }

            _oldSprite = Sprite;
        }
    }
}
