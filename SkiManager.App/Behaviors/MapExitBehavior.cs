using System;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class MapExitBehavior : ReactiveBehavior, IMapExit
    {
        private IDisposable _subscription;

        protected override void Loaded()
        {
            _subscription = ChildEnter.Subscribe(OnChildEnter);
        }

        protected override void Unloading()
        {
            _subscription.Dispose();
        }

        private void OnChildEnter(ChildEnterEngineEventArgs args)
        {
            if (!args.EnteringChild.HasBehavior<CarBehavior>())
            {
                return;
            }

            var car = args.EnteringChild.GetBehavior<CarBehavior>();
            if (car.HasBeenParkedAtSomeTime)
            {
                Entity.Level.Destroy(args.EnteringChild);
            }
        }
    }
}
