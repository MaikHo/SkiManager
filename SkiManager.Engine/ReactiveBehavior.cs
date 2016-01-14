using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
            Draw = Engine.Current.Events.Draw.Where(_ => CanReceiveEvent(_, true)).Publish().RefCount();
            Update = Engine.Current.Events.Update.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            EarlyCreateResources = Engine.Current.Events.EarlyCreateResources.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            CreateResources = Engine.Current.Events.CreateResources.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            PointerMoved = Engine.Current.Events.PointerMoved.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            ChildEnter = Entity.ChildEnter.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            ChildLeave = Entity.ChildLeave.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();
            ParentChanged = Entity.ParentChanged.Where(_ => CanReceiveEvent(_, false)).Publish().RefCount();

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

        private bool CanReceiveEvent(EngineEventArgs args, bool ignoreEnginePause) => IsEffectivelyEnabled && (ignoreEnginePause || !Engine.Current.Status.IsPaused);
    }
}
    ;