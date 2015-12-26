using System.Collections.Generic;
using System.Linq;

namespace SkiManager.Engine
{
    public sealed class Level
    {
        public Entity RootEntity { get; } = new Entity { Name = "Root" };

        private readonly List<Entity> _entities = new List<Entity>();
        public IReadOnlyList<Entity> Entities => _entities.AsReadOnly();

        internal IDictionary<Entity, IList<Entity>> ChildrenLookup { get; } = new Dictionary<Entity, IList<Entity>>();

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
                behavior.Unloading();
                behavior.Destroyed();
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
                    behavior.Unloading();
                }
            }
        }

        internal void Loaded()
        {
            foreach (var entity in Entities)
            {
                foreach (var behavior in entity.Behaviors)
                {
                    behavior.Loaded();
                }
                entity.IsLoaded = true;
            }
        }
    }
}
