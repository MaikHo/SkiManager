using System;
using System.Reactive.Linq;

namespace SkiManager.Engine
{
    public abstract class ReactiveBehavior
    {
        public Entity Entity { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsEffectivelyEnabled => IsEnabled && (Entity?.IsEnabled ?? false);


        protected IObservable<EngineDrawEventArgs> Draw { get; }
        protected IObservable<EngineUpdateEventArgs> Update { get; }
        protected IObservable<EnginePointerMovedEventArgs> PointerMoved { get; }

        protected ReactiveBehavior()
        {
            Draw = Engine.Current.Events.Draw.Where(_ => IsEffectivelyEnabled).Publish().RefCount();
            Update = Engine.Current.Events.Update.Where(_ => IsEffectivelyEnabled).Publish().RefCount();
            PointerMoved = Engine.Current.Events.PointerMoved.Where(_ => IsEffectivelyEnabled).Publish().RefCount();
        }

        internal void Attach(Entity entity)
        {
            Entity = entity;
        }

        internal void Detach(Entity entity)
        {
            Entity = null;
        }
    }
}
