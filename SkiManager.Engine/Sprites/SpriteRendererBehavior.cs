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

                e.DrawingSession.DrawImage(sprite.Image, dipsRect);
            }
        }
    }
}
