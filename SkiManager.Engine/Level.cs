using System.Collections.Generic;
using SkiManager.Engine.Behaviors;
using System.Linq;

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
        public Entity Instantiate(Entity entity, Entity newEntityParent = null) => Instantiate(entity, newEntityParent, true);

        private Entity Instantiate(Entity entity, Entity newEntityParent, bool mustBeCloned)
        {
            var newEntity = entity;

            if (mustBeCloned)
            {
                newEntity = entity.Clone();
                newEntity.Name += " (Clone)";
                AddEntity(newEntity);
            }
            else
            {
                if (!Entities.Contains(newEntity))
                {
                    AddEntity(newEntity);
                }
            }

            // set parent and position
            newEntity.SetParent(newEntityParent, Reason.EngineInternal);
            if (newEntityParent != null && newEntityParent.HasBehavior<TransformBehavior>())
            {
                newEntity.GetBehavior<TransformBehavior>().Position = newEntityParent.GetBehavior<TransformBehavior>().Position;
            }
            if (newEntityParent?.IsLoaded ?? newEntityParent == null)
                newEntity.IsLoaded = true;

            // restore behavior attachment
            foreach (var behavior in newEntity.Behaviors)
            {
                behavior.Attach(newEntity);

                if (newEntityParent?.IsLoaded ?? newEntityParent == null)
                    behavior.LoadedInternal();
            }

            // recurse for each child, but do not clone them (they are already)
            var children = newEntity.Children.ToList();
            newEntity._children.Clear();
            foreach (var child in children)
            {
                Instantiate(child, newEntity, false);
            }

            return newEntity;
        }

        public void Destroy(Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            entity.IsEnabled = false;
            entity.SetParent(null, Reason.EngineInternal);

            // destroy children first
            foreach (var child in entity.Children)
            {
                Destroy(child);
            }

            // then remove this entity from the list of entities, unload behaviors and destroy
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
            foreach (var entity in Entities.Where(_ => !_.IsLoaded))
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
