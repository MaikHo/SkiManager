using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresBehavior(typeof(MovableBehavior))]
    public sealed class CarBehavior : TransporterBaseBehavior
    {
        private IDisposable _updateSubscription;
        private IDisposable _targetReachedSubscription;
        private bool _hasBeenParkedAtSomeTime;

        protected override void Loaded()
        {
            _updateSubscription = Update.Subscribe(OnUpdate);
            _targetReachedSubscription = Entity.GetBehavior<MovableBehavior>().TargetReached.Subscribe(OnTargetReached);
        }

        protected override void Unloading()
        {
            _updateSubscription.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            _hasBeenParkedAtSomeTime |= IsParked;
        }

        private void OnTargetReached(TargetReachedEngineEventArgs args)
        {
            if (!IsParked && !_hasBeenParkedAtSomeTime)
            {
                // this car only spawned, it has never been parked ->
            }
        }

        internal void SetTargetToNextPointTowardsParkingLot()
        {

        }

        internal void SetTargetToNextPointTowardsRandomOfClosestThreeMapExits()
        {

        }
    }
}
