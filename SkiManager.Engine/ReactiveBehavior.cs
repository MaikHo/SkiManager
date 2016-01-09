using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace SkiManager.Engine
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ReactiveBehavior
    {
        private readonly List<IDisposable> _trackedSubscriptions = new List<IDisposable>();

        [JsonProperty]
        public Entity Entity { get; private set; }

        [JsonProperty]
        public bool IsEnabled { get; set; } = true;
        
        public bool IsEffectivelyEnabled => IsEnabled && (Entity?.IsEffectivelyEnabled ?? false);


        protected IObservable<EngineDrawEventArgs> Draw { get; private set; }
        protected IObservable<EngineUpdateEventArgs> Update { get; private set; }
        protected IObservable<EngineCreateResourcesEventArgs> EarlyCreateResources { get; private set; }
        protected IObservable<EngineCreateResourcesEventArgs> CreateResources { get; private set; }
        protected IObservable<EnginePointerMovedEventArgs> PointerMoved { get; private set; }
        protected IObservable<ChildEnterEngineEventArgs> ChildEnter { get; private set; }
        protected IObservable<ChildLeaveEngineEventArgs> ChildLeave { get; private set; }
        protected IObservable<ParentChangedEngineEventArgs> ParentChanged { get; private set; }

        internal void LoadedInternal()
        {
            Draw = Engine.Current.Events.Draw.Where(CanReceiveEvent).Publish().RefCount();
            Update = Engine.Current.Events.Update.Where(CanReceiveEvent).Publish().RefCount();
            EarlyCreateResources = Engine.Current.Events.EarlyCreateResources.Where(CanReceiveEvent).Publish().RefCount();
            CreateResources = Engine.Current.Events.CreateResources.Where(CanReceiveEvent).Publish().RefCount();
            PointerMoved = Engine.Current.Events.PointerMoved.Where(CanReceiveEvent).Publish().RefCount();
            ChildEnter = Entity.ChildEnter.Where(CanReceiveEvent).Publish().RefCount();
            ChildLeave = Entity.ChildLeave.Where(CanReceiveEvent).Publish().RefCount();
            ParentChanged = Entity.ParentChanged.Where(CanReceiveEvent).Publish().RefCount();

            _trackedSubscriptions.Clear();
            var args = new BehaviorLoadedEventArgs(_trackedSubscriptions);

            Loaded(args);
        }

        protected virtual void Loaded(BehaviorLoadedEventArgs args)
        {
        }

        internal void UnloadingInternal()
        {
            Unloading();

            foreach (var subscription in _trackedSubscriptions)
            {
                subscription?.Dispose();
            }
            _trackedSubscriptions.Clear();
        }

        protected virtual void Unloading()
        {
        }

        internal void DestroyedInternal()
        {
            Destroyed();
        }

        protected virtual void Destroyed()
        {
        }

        internal void Attach(Entity entity)
        {
            Entity = entity;
        }

        internal void Detach(Entity entity)
        {
            Entity = null;
        }

        private bool CanReceiveEvent(EngineEventArgs args) => IsEffectivelyEnabled;
    }
}
    ;