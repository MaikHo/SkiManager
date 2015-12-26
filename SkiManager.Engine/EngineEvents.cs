using System;
using System.Reactive.Subjects;

using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    internal sealed class EngineEvents : IEngineEvents
    {
        private Engine _engine;

        public IObservable<EngineDrawEventArgs> Draw { get; private set; }

        public IObservable<EngineUpdateEventArgs> Update { get; private set; }

        public IObservable<EngineCreateResourcesEventArgs> CreateResources { get; private set; }

        public IObservable<EnginePointerMovedEventArgs> PointerMoved { get; private set; }

        private EngineEvents()
        {
        }

        public static EngineEvents Attach(Engine engine, CanvasAnimatedControl control)
        {
            var events = new EngineEvents();
            events._engine = engine;

            var draw = new Subject<EngineDrawEventArgs>();
            events.Draw = draw;
            control.Draw += (_, __) => draw.OnNext(new EngineDrawEventArgs(Engine.Current, _, __));

            var update = new Subject<EngineUpdateEventArgs>();
            events.Update = update;
            control.Update += (_, __) => update.OnNext(new EngineUpdateEventArgs(Engine.Current, _, __));

            var createResources = new Subject<EngineCreateResourcesEventArgs>();
            events.CreateResources = createResources;
            control.CreateResources += (sender, e) =>
            {
                var args = new EngineCreateResourcesEventArgs(Engine.Current, sender, e);
                createResources.OnNext(args);
                e.TrackAsyncAction(args.Tasks.CompleteAllAsync().AsAsyncAction());
            };

            var pointerMoved = new Subject<EnginePointerMovedEventArgs>();
            events.PointerMoved = pointerMoved;
            control.PointerMoved += (_, __) => pointerMoved.OnNext(new EnginePointerMovedEventArgs(Engine.Current, _ as ICanvasAnimatedControl, __));

            return events;
        }
    }
}
