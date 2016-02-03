using System;
using System.Collections.Generic;
using SkiManager.Engine;
using SkiManager.Engine.Interfaces;

namespace SkiManager.App.Interfaces
{
    public interface IWaitingQueue : IHasEntity
    {
        int MaxQueueSize { get; }

        int QueueSize { get; }

        bool HasWaitingEntities { get; }

        IObservable<Entity> WaitingEntityArrived { get; }

        IEnumerable<Entity> GetEntitiesFromQueue(int count = 1);
    }
}
