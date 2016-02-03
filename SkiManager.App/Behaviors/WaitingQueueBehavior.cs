using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Interfaces;

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
        private readonly List<Entity> _queue = new List<Entity>();
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
            
            _queue.Add(args.EnteringChild);
            _waitingEntityArrivedSubject.OnNext(args.EnteringChild);
        }

        public IEnumerable<Entity> GetEntitiesFromQueue(int count = 1)
        {
            for (var i = 0; i < count && _queue.Count > 0; i++)
            {
                var current = _queue.First();
                _queue.RemoveAt(0);
                yield return current;
            }
        }
    }
}
