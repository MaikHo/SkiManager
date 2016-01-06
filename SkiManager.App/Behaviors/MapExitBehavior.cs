using System;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class MapExitBehavior : ReactiveBehavior, IMapExit
    {
        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ChildEnter.Subscribe(OnChildEnter));
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
