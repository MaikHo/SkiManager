using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class WaitingQueueBehavior : ReactiveBehavior, IWaitingQueue
    {
        [JsonProperty]
        public int MaxQueueSize { get; set; }
        public int QueueSize => _queue.Count;
        public bool HasWaitingEntities => QueueSize > 0;
        public IObservable<Entity> WaitingEntityArrived => _waitingEntityArrivedSubject.AsObservable();

        [JsonProperty]
        private List<Entity> _queue = new List<Entity>();
        private readonly Subject<Entity> _waitingEntityArrivedSubject = new Subject<Entity>();

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ChildEnter.Subscribe(OnChildEnter));
        }

        protected override void Destroyed()
        {
            _waitingEntityArrivedSubject.Dispose();
        }

        private void OnChildEnter(ChildEnterEngineEventArgs args)
        {
            if (_queue.Count >= MaxQueueSize)
            {
                // no space
                args.EnteringChild.SetParent(args.OldParent, Reasons.NoSpace.InWaitingQueue);
                return;
            }

            args.EnteringChild.IsEnabled = false;
            _queue.Add(args.EnteringChild);
            _waitingEntityArrivedSubject.OnNext(args.EnteringChild);
        }

        public IReadOnlyList<Entity> GetDisabledEntitiesFromQueue(int count = 1) => _queue.Take(count).ToList().AsReadOnly();
    }
}
