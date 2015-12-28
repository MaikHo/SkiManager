using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using System.Threading.Tasks;

namespace SkiManager.Engine
{
    internal sealed class EngineEvents : IEngineEvents
    {
        private Engine _engine;

        public IObservable<EngineDrawEventArgs> Draw { get; private set; }

        public IObservable<EngineUpdateEventArgs> Update { get; private set; }

        public IObservable<EngineCreateResourcesEventArgs> EarlyCreateResources { get; private set; }

        public IObservable<EngineCreateResourcesEventArgs> CreateResources { get; private set; }

        public IObservable<EnginePointerMovedEventArgs> PointerMoved { get; private set; }

        private EngineEvents()
        {
        }

        public static EngineEvents Attach(Engine engine, CanvasVirtualControl control)
        {
            var events = new EngineEvents();
            events._engine = engine;

            var draw = new Subject<EngineDrawEventArgs>();
            events.Draw = draw;
            control.RegionsInvalidated += (s, e) =>
            {
                var args = new EngineDrawEventArgs(Engine.Current, s, e);
                foreach (var region in e.InvalidatedRegions)
                {
                    using (var drawingSession = control.CreateDrawingSession(region))
                    {
                        args.DrawingSession = drawingSession;
                        draw.OnNext(args);
                    }
                }
            };

            // TODO: Correct arguments, make interval configurable
            events.Update = Observable.Interval(TimeSpan.FromMilliseconds(100)).Select(_ => new EngineUpdateEventArgs(Engine.Current, null, null)).Publish().RefCount();

            var createResources = new Subject<EngineCreateResourcesEventArgs>();
            events.CreateResources = createResources;
            var earlyCreateResources = new Subject<EngineCreateResourcesEventArgs>();
            events.EarlyCreateResources = earlyCreateResources;

            control.CreateResources += (sender, e) =>
            {
                if (e.Reason == CanvasCreateResourcesReason.DpiChanged)
                    return;

                var task = OnCreateResources(earlyCreateResources, createResources, sender, e);
                e.TrackAsyncAction(task.AsAsyncAction());
            };


            var pointerMoved = new Subject<EnginePointerMovedEventArgs>();
            events.PointerMoved = pointerMoved;
            control.PointerMoved += (s, e) => pointerMoved.OnNext(new EnginePointerMovedEventArgs(Engine.Current, s as CanvasVirtualControl, e));

            return events;
        }

        private static async Task OnCreateResources(Subject<EngineCreateResourcesEventArgs> earlyCreateResources, Subject<EngineCreateResourcesEventArgs> createResources, CanvasVirtualControl sender, CanvasCreateResourcesEventArgs e)
        {
            var args = new EngineCreateResourcesEventArgs(Engine.Current, sender, e);
            earlyCreateResources.OnNext(args);
            await args.Tasks.CompleteAllAsync();

            args = new EngineCreateResourcesEventArgs(Engine.Current, sender, e);
            createResources.OnNext(args);
            await args.Tasks.CompleteAllAsync();
        }
    }
}
