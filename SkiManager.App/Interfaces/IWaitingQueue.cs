using System;
using System.Collections.Generic;
using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IWaitingQueue : IHasEntity
    {
        int MaxQueueSize { get; }

        int QueueSize { get; }

        bool HasWaitingEntities { get; }

        IObservable<Entity> WaitingEntityArrived { get; }

        IReadOnlyList<Entity> GetDisabledEntitiesFromQueue(int count = 1);
    }
}
