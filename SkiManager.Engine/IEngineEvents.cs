using System;

namespace SkiManager.Engine
{
    public interface IEngineEvents
    {
        IObservable<EngineDrawEventArgs> Draw { get; }

        IObservable<EngineUpdateEventArgs> Update { get; }

        IObservable<EnginePointerMovedEventArgs> PointerMoved { get; }
    }
}
