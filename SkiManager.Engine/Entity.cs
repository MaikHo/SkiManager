using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;

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

        internal Subject<ChildEnterEngineEventArgs> ChildEnter { get; } = new Subject<ChildEnterEngineEventArgs>();

        internal Subject<ChildLeaveEngineEventArgs> ChildLeave { get; } = new Subject<ChildLeaveEngineEventArgs>();

        internal Subject<ParentChangedEngineEventArgs> ParentChanged { get; } = new Subject<ParentChangedEngineEventArgs>();

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

        internal void Destroyed()
        {
            ChildEnter.Dispose();
            ChildLeave.Dispose();
            ParentChanged.Dispose();
        }

        public void SetParent(Entity newParent)
        {
            var oldParent = Parent;
            if (oldParent != null)
            {
                oldParent.ChildLeave.OnNext(new ChildLeaveEngineEventArgs(Engine.Current, this));
                if (Parent != oldParent)
                {
                    // Parent has been set in a childleave handler, therefore return
                    return;
                }
                Level.ChildrenLookup[Parent].Remove(this);
            }

            Parent = newParent;

            if (!Level.ChildrenLookup.ContainsKey(Parent))
            {
                Level.ChildrenLookup.Add(Parent, new List<Entity>());
            }
            Level.ChildrenLookup[Parent].Add(this);
            newParent.ChildEnter.OnNext(new ChildEnterEngineEventArgs(Engine.Current, this, oldParent));
            if (Parent != newParent)
            {
                // Parent has been set in a childenter handler, therefore return
                return;
            }
            ParentChanged.OnNext(new ParentChangedEngineEventArgs(Engine.Current, oldParent, newParent));
        }
    }
}
