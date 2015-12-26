using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections;

namespace SkiManager.Engine.Behaviors
{
    public class SpriteManagerBehavior : ReactiveBehavior
    {
        private IDisposable _createResourcesSubscription;

        public SpriteCollection Sprites { get; } = new SpriteCollection();

        protected internal override void Loaded()
        {
            _createResourcesSubscription = CreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e)));
        }

        protected internal override void Unloading()
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

        public void AddSprite(string id, Uri source, Vector2 size)
        {
            if (_sprites.ContainsKey(id))
                throw new ArgumentException($"Sprite-ID '{id}' is already in use");

            var sprite = new Sprite(id, source, size);
            _sprites.Add(id, sprite);
        }

        public bool RemoveSprite(string id)
        {
            return _sprites.Remove(id);
        }

        public IEnumerator<Sprite> GetEnumerator() => _sprites.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Sprite this[string id]
        {
            get { return _sprites[id]; }
        }
    }

    
    public class SpriteRenderer : ReactiveBehavior
    {
        private IDisposable _drawSubscription;

        public SpriteReference Sprite { get; set; }

        protected internal override void Loaded()
        {
            _drawSubscription = Draw.Subscribe(OnDraw);
        }

        protected internal override void Unloading()
        {
            _drawSubscription.Dispose();
        }

        private void OnDraw(EngineDrawEventArgs e)
        {
            var transform = Entity.GetBehavior<TransformBehavior>();

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
        public string Id { get; private set; }

        public static implicit operator SpriteReference(string id)
            => new SpriteReference { Id = id };
    }
}
