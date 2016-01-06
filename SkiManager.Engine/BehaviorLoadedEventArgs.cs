using System;
using System.Collections.Generic;

namespace SkiManager.Engine
{
    public sealed class BehaviorLoadedEventArgs
    {
        private readonly IList<IDisposable> _trackedSubscriptions;

        public BehaviorLoadedEventArgs(IList<IDisposable> trackedSubscriptionsList)
        {
            _trackedSubscriptions = trackedSubscriptionsList;
        }

        /// <summary>
        /// Tracks the given subscription and automatically disposes it when the behavior is unloaded.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        public void TrackSubscription(IDisposable subscription)
        {
            _trackedSubscriptions.Add(subscription);
        }
    }
}
