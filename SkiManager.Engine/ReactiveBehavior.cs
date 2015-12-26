using System;
using System.Reactive.Linq;

namespace SkiManager.Engine
{
    public abstract class ReactiveBehavior
    {
        public Entity Entity { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsEffectivelyEnabled => IsEnabled && (Entity?.IsEffectivelyEnabled ?? false);


        protected IObservable<EngineDrawEventArgs> Draw { get; private set; }
        protected IObservable<EngineUpdateEventArgs> Update { get; private set; }
        protected IObservable<EnginePointerMovedEventArgs> PointerMoved { get; private set; }
        protected IObservable<ChildEnterEngineEventArgs> ChildEnter { get; private set; }
        protected IObservable<ChildLeaveEngineEventArgs> ChildLeave { get; private set; }
        protected IObservable<ParentChangedEngineEventArgs> ParentChanged { get; private set; }

        internal void LoadedInternal()
        {
            Draw = Engine.Current.Events.Draw.Where(CanReceiveEvent).Publish().RefCount();
            Update = Engine.Current.Events.Update.Where(CanReceiveEvent).Publish().RefCount();
            CreateResources = Engine.Current.Events.CreateResources.Where(CanReceiveEvent).Publish().RefCount();
            PointerMoved = Engine.Current.Events.PointerMoved.Where(CanReceiveEvent).Publish().RefCount();
            ChildEnter = Entity.ChildEnter.Where(CanReceiveEvent).Publish().RefCount();
            ChildLeave = Entity.ChildLeave.Where(CanReceiveEvent).Publish().RefCount();
            ParentChanged = Entity.ParentChanged.Where(CanReceiveEvent).Publish().RefCount();

            Loaded();
        }

        protected virtual void Loaded()
        {
        }

        internal void UnloadingInternal()
        {
            Unloading();
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