using System.Collections.Generic;
using SkiManager.Engine.Behaviors;

namespace SkiManager.Engine
{
    public sealed class Level
    {
        public Entity RootEntity { get; }

        private readonly List<Entity> _entities;
        public IReadOnlyList<Entity> Entities => _entities.AsReadOnly();

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

        /// <summary>
        /// Creates a clone of the specified <see cref="Entity"/> and sets
        /// its parent to the specified parent entity.
        /// If the parent entity is loaded the new entity will also be loaded.
        /// </summary>
        /// <param name="entity">Entity to be cloned</param>
        /// <param name="newEntityParent">Parent of the instantiated entity</param>
        /// <returns>The entity clone</returns>
        public Entity Instantiate(Entity entity, Entity newEntityParent = null)
        {
            var newEntity = entity.Clone();
            newEntity.Name += " (Clone)";
            newEntity.SetParent(newEntityParent);
            if (newEntityParent != null && newEntityParent.HasBehavior<TransformBehavior>())
            {
                newEntity.GetBehavior<TransformBehavior>().Position = newEntityParent.GetBehavior<TransformBehavior>().Position;
            }
            AddEntity(newEntity);
            // restore behavior attachment
            foreach (var behavior in newEntity.Behaviors)
            {
                behavior.Attach(newEntity);

                if (newEntityParent?.IsLoaded ?? false)
                    behavior.LoadedInternal();
            }

            if (newEntityParent?.IsLoaded ?? false)
                newEntity.IsLoaded = true;

            // TODO: Deep loading of all descendants and their behaviors

            return newEntity;
        }
        
        public void Destroy(Entity entity)
        {
            // TODO: do this hierarchically for the whole subtree
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
