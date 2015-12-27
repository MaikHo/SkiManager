namespace SkiManager.Engine.Sprites
{
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

        public override string ToString()
            => Id;

        /// <summary>
        /// Resolves the <see cref="SpriteReference"/> by looking up the
        /// <see cref="Sprite"/> in the <see cref="SpriteManagerBehavior"/>
        /// at the root of the specified <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">
        /// Entity where the root contains the <see cref="SpriteManagerBehavior"/>
        /// from which the <see cref="Sprite"/> is obtained.
        /// </param>
        /// <returns></returns>
        public Sprite Resolve(Entity entity)
        {
            var spriteManager = entity.Level.RootEntity.GetBehavior<SpriteManagerBehavior>();
            return spriteManager?.Sprites[this];
        }
    }
}
