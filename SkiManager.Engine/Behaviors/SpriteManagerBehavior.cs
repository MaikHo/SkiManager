using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections;
using Windows.Foundation;
using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Behaviors
{
    public class SpriteManagerBehavior : ReactiveBehavior
    {
        private IDisposable _createResourcesSubscription;

        public SpriteCollection Sprites { get; } = new SpriteCollection();

        protected override void Loaded()
        {
            _createResourcesSubscription = CreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e)));
        }

        protected override void Unloading()
        {
            _createResourcesSubscription.Dispose();
        }

        private async Task OnCreateResourcesAsync(EngineCreateResourcesEventArgs e)
        {
            // Load all registered sprites
            await Task.WhenAll(Sprites.Select(sprite => sprite.LoadAsync(e.Sender)));
        }
    }

    public class SpriteCollection : IEnumerable<Sprite>
    {
        private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public SpriteCollection()
        {
        }

        public void Add(string id, Uri source, Vector2 size)
        {
            if (_sprites.ContainsKey(id))
                throw new ArgumentException($"Sprite-ID '{id}' is already in use");

            var sprite = new Sprite(id, source, size);
            _sprites.Add(id, sprite);
        }

        public bool Remove(string id)
        {
            return _sprites.Remove(id);
        }

        public IEnumerator<Sprite> GetEnumerator() => _sprites.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Sprite this[string id]
        {
            get
            {
                Sprite sprite;
                return _sprites.TryGetValue(id, out sprite) ? sprite : null;
            }
        }

        public Sprite this[SpriteReference spriteRef] => this[spriteRef.Id];
    }

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
            var spriteManager = Entity.Level.RootEntity.GetBehavior<SpriteManagerBehavior>();
            var sprite = spriteManager?.Sprites[Sprite];

            if (transform != null && coordinateSystem != null && sprite != null)
            {
                var worldPos = transform.Position;

                var worldRect = new Rect(
                    worldPos.X - sprite.Size.X / 2,
                    worldPos.Y - sprite.Size.Y / 2,
                    sprite.Size.X,
                    sprite.Size.Y);

                var dipsRect = coordinateSystem.TransformToDips(worldRect);

                e.Arguments.DrawingSession.DrawImage(sprite.Image, dipsRect);
            }
        }
    }

    public class Sprite
    {
        public string Id { get; }
        public Uri Source { get; }

        /// <summary>
        /// Virtual size in world coordinates (meters).
        /// </summary>
        public Vector2 Size { get; }

        public CanvasBitmap Image { get; private set; }

        public Sprite(string id, Uri source, Vector2 size)
        {
            Id = id;
            Source = source;
            Size = size;
        }

        internal async Task LoadAsync(ICanvasResourceCreator resourceCreator)
        {
            Image = await CanvasBitmap.LoadAsync(resourceCreator, Source);
        }
    }

    public struct SpriteReference
    {
        public static SpriteReference Empty { get; } = new SpriteReference();

        public string Id { get; private set; }

        public static implicit operator SpriteReference(string id)
            => new SpriteReference { Id = id };

        public static bool operator ==(SpriteReference a, SpriteReference b)
            => Equals(a, b);

        public static bool operator !=(SpriteReference a, SpriteReference b)
            => !Equals(a, b);

        public override bool Equals(object obj)
            => obj.GetType() == typeof(SpriteReference) && ((SpriteReference)obj).Id == Id;

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
