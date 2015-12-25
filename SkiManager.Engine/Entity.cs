using System.Collections.Generic;
using System.Diagnostics;

namespace SkiManager.Engine
{
    [DebuggerDisplay("Entity \"{Name}\"")]
    public sealed class Entity
    {
        private readonly List<ReactiveBehavior> _behaviors = new List<ReactiveBehavior>();

        public IReadOnlyList<ReactiveBehavior> Behaviors => _behaviors;

        public bool IsEffectivelyEnabled => IsEnabled && IsLoaded && !IsDestroyed && (Parent?.IsEffectivelyEnabled ?? true);

        public bool IsEnabled { get; set; } = true;

        public string Name { get; set; } = nameof(Entity);

        public Entity Parent { get; private set; }

        public Level Level { get; internal set; }

        public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

        internal bool IsLoaded { get; set; }

        internal bool IsDestroyed { get; set; }

        public void AddBehavior(ReactiveBehavior behavior)
        {
            behavior.Attach(this);
            _behaviors.Add(behavior);
        }

        public void RemoveBehavior(ReactiveBehavior behavior)
        {
            _behaviors.Remove(behavior);
            behavior.Detach(this);
        }

        public void MoveToParent(Entity newParent, Level containingLevel)
        {
            if (Parent != null)
            {
                containingLevel.ChildrenLookup[Parent].Remove(this);
            }

            Parent = newParent;

            if (!containingLevel.ChildrenLookup.ContainsKey(Parent))
            {
                containingLevel.ChildrenLookup.Add(Parent, new List<Entity>());
            }
            containingLevel.ChildrenLookup[Parent].Add(this);
        }
    }
}
