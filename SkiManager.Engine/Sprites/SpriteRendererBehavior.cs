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
            var coordinateSystem = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var sprite = Sprite.Resolve(Entity);

            if (transform != null && coordinateSystem != null && sprite != null)
            {
                var worldPos = transform.Position;

                var worldRect = new Rect(
                    worldPos.X - sprite.Size.X / 2,
                    worldPos.Y - sprite.Size.Y / 2,
                    sprite.Size.X,
                    sprite.Size.Y);

                var dipsRect = coordinateSystem.TransformToDips(worldRect);

                e.DrawingSession.DrawImage(sprite.Image, dipsRect);
            }
        }
    }
}
