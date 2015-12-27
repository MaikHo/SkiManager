using System.Collections.Generic;

namespace SkiManager.Engine
{
    public sealed class Level
    {
        public Entity RootEntity { get; }

        private readonly List<Entity> _entities;
        public IReadOnlyList<Entity> Entities => _entities.AsReadOnly();

        internal IDictionary<Entity, IList<Entity>> ChildrenLookup { get; } = new Dictionary<Entity, IList<Entity>>();

        public Level()
        {
            RootEntity = new Entity { Name = "Root", Level = this };
            _entities = new List<Entity> { RootEntity };
        }

        public void AddEntity(Entity entity)
        {
            entity.Level = this;
            _entities.Add(entity);
        }

        public void Destroy(Entity entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
            foreach (var behavior in entity.Behaviors)
            {
                behavior.UnloadingInternal();
                behavior.DestroyedInternal();
            }
            entity.Destroyed();
            entity.IsDestroyed = true;
        }

        internal void Unloading()
        {
            foreach (var entity in Entities)
            {
                entity.IsLoaded = false;
                foreach (var behavior in entity.Behaviors)
                {
                    behavior.UnloadingInternal();
                }
            }
        }

        internal void Loaded()
        {
            foreach (var entity in Entities)
            {
                foreach (var behavior in entity.Behaviors)
                {
                    behavior.LoadedInternal();
                }
                entity.IsLoaded = true;
            }
        }
    }
}
