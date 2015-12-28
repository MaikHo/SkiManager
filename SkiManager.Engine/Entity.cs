﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace SkiManager.Engine
{
    [DebuggerDisplay("Entity \"{Name}\"")]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class Entity
    {
        [JsonProperty]
        private readonly List<ReactiveBehavior> _behaviors = new List<ReactiveBehavior>();

        public IReadOnlyList<ReactiveBehavior> Behaviors => _behaviors;

        public bool IsEffectivelyEnabled => IsEnabled && IsLoaded && !IsDestroyed && (Parent?.IsEffectivelyEnabled ?? true);

        [JsonProperty]
        public bool IsEnabled { get; set; } = true;

        [JsonProperty]
        public string Name { get; set; } = nameof(Entity);

        [JsonProperty]
        public Entity Parent { get; private set; }

        [JsonProperty]
        private readonly List<Entity> _children = new List<Entity>();
        public IReadOnlyList<Entity> Children => _children.AsReadOnly();

        [JsonProperty]
        public Level Level { get; internal set; }

        [JsonProperty]
        public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

        [JsonProperty]
        internal bool IsLoaded { get; set; }

        [JsonProperty]
        internal bool IsDestroyed { get; set; }

        [JsonProperty]
        internal Subject<ChildEnterEngineEventArgs> ChildEnter { get; } = new Subject<ChildEnterEngineEventArgs>();

        [JsonProperty]
        internal Subject<ChildLeaveEngineEventArgs> ChildLeave { get; } = new Subject<ChildLeaveEngineEventArgs>();

        [JsonProperty]
        internal Subject<ParentChangedEngineEventArgs> ParentChanged { get; } = new Subject<ParentChangedEngineEventArgs>();

        public T AddBehavior<T>(T behavior) where T : ReactiveBehavior
        {
            behavior.Attach(this);
            _behaviors.Add(behavior);
            return behavior;
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
                Parent._children.Remove(this);
            }

            Parent = newParent;

            if (Parent != null)
            {
                Parent._children.Add(this);
                newParent.ChildEnter.OnNext(new ChildEnterEngineEventArgs(Engine.Current, this, oldParent));
                if (Parent != newParent)
                {
                    // Parent has been set in a childenter handler, therefore return
                    return;
                }
            }
            ParentChanged.OnNext(new ParentChangedEngineEventArgs(Engine.Current, oldParent, newParent));
        }

        internal Entity Clone()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All
            };
            var objectString = JsonConvert.SerializeObject(this, Formatting.None, settings);
            return JsonConvert.DeserializeObject<Entity>(objectString, settings);
        }
    }
}
