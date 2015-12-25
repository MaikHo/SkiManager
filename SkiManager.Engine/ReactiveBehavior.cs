using System;
using System.Reactive.Linq;

namespace SkiManager.Engine
{
    public abstract class ReactiveBehavior
    {
        public Entity Entity { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsEffectivelyEnabled => IsEnabled && (Entity?.IsEffectivelyEnabled ?? false);


        protected IObservable<EngineDrawEventArgs> Draw { get; }
        protected IObservable<EngineUpdateEventArgs> Update { get; }
        protected IObservable<EnginePointerMovedEventArgs> PointerMoved { get; }

        protected ReactiveBehavior()
        {
            Draw = Engine.Current.Events.Draw.Where(CanReceiveEvent).Publish().RefCount();
            Update = Engine.Current.Events.Update.Where(CanReceiveEvent).Publish().RefCount();
            PointerMoved = Engine.Current.Events.PointerMoved.Where(CanReceiveEvent).Publish().RefCount();
        }

        protected internal virtual void Loaded()
        { }

        protected internal virtual void Unloading()
        { }

        protected internal virtual void Destroyed()
        { }

        internal void Attach(Entity entity)
        {
            Entity = entity;
        }

        internal void Detach(Entity entity)
        {
            Entity = null;
        }

        private bool CanReceiveEvent(EngineEventArgs args)
        {
            return IsEffectivelyEnabled;
        }
    }
}
