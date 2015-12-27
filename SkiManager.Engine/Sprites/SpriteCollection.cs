using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace SkiManager.Engine.Sprites
{
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

        /// <summary>
        /// Returns the <see cref="Sprite"/> with the specified
        /// reference or null if no such sprite exists.
        /// </summary>
        /// <param name="spriteRef">Sprite reference</param>
        /// <returns>Sprite</returns>
        public Sprite this[SpriteReference spriteRef]
        {
            get
            {
                Sprite sprite;
                return _sprites.TryGetValue(spriteRef.Id, out sprite) ? sprite : null;
            }
        }
    }
}
