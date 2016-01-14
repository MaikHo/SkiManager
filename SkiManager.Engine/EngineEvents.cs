using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace SkiManager.Engine
{
    internal sealed class EngineEvents : IEngineEvents, IDisposable
    {
        private Engine _engine;
        private readonly HashSet<RenderLayer> _renderLayers = new HashSet<RenderLayer>();

        private readonly Subject<EngineDrawEventArgs> _draw = new Subject<EngineDrawEventArgs>();
        public IObservable<EngineDrawEventArgs> Draw => _draw.AsObservable();

        public IObservable<EngineUpdateEventArgs> Update { get; private set; }

        private readonly Subject<EngineCreateResourcesEventArgs> _earlyCreateResources = new Subject<EngineCreateResourcesEventArgs>();
        public IObservable<EngineCreateResourcesEventArgs> EarlyCreateResources => _earlyCreateResources.AsObservable();

        private readonly Subject<EngineCreateResourcesEventArgs> _createResources = new Subject<EngineCreateResourcesEventArgs>();
        public IObservable<EngineCreateResourcesEventArgs> CreateResources => _createResources.AsObservable();

        private readonly Subject<EnginePointerMovedEventArgs> _pointerMoved = new Subject<EnginePointerMovedEventArgs>();
        public IObservable<EnginePointerMovedEventArgs> PointerMoved => _pointerMoved.AsObservable();

        private EngineEvents()
        {
        }

        internal static EngineEvents Attach(Engine engine, CanvasVirtualControl control)
        {
            var events = new EngineEvents
            {
                _engine = engine
            };

            // register default RenderLayers
            foreach (var layer in RenderLayers.GetAllLayers())
            {
                events.RegisterRenderLayer(layer);
            }

            // attach to events
            control.RegionsInvalidated += events.RaiseDrawOnRegionsInvalidated;

            control.CreateResources += events.RaiseCreateResources;

            control.PointerMoved += events.RaisePointerMoved;

            // force regular draw
            // TODO make configurable
            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(async _ => await control.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, control.Invalidate));

            // create update observable
            // TODO: Correct arguments, make interval configurable
            events.Update = Observable.Interval(TimeSpan.FromMilliseconds(100)).Select(_ => new EngineUpdateEventArgs(Engine.Current, null, null)).Publish().RefCount();

            return events;
        }

        private void RaisePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _pointerMoved.OnNext(new EnginePointerMovedEventArgs(Engine.Current, sender as CanvasVirtualControl, e));
        }

        private void RaiseCreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.DpiChanged)
                return;

            var task = OnCreateResources(_earlyCreateResources, _createResources, sender, args);
            args.TrackAsyncAction(task.AsAsyncAction());
        }

        private void RaiseDrawOnRegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            if (!_renderLayers.Any())
            {
                return;
            }

            foreach (var region in args.InvalidatedRegions)
            {
                using (var drawingSession = sender.CreateDrawingSession(region))
                {
                    // raise for each RenderLayer in ascending order (higher index -> later rendering)
                    foreach (var renderLayer in _renderLayers.OrderBy(_ => _.Index))
                    {
                        var engineArgs = new EngineDrawEventArgs(Engine.Current, sender, args, renderLayer)
                        {
                            DrawingSession = drawingSession
                        };
                        _draw.OnNext(engineArgs);
                    }
                }
            }
        }

        private static async Task OnCreateResources(Subject<EngineCreateResourcesEventArgs> earlyCreateResources, Subject<EngineCreateResourcesEventArgs> createResources, CanvasVirtualControl sender, CanvasCreateResourcesEventArgs e)
        {
            var args = new EngineCreateResourcesEventArgs(Engine.Current, sender, e);
            earlyCreateResources.OnNext(args);
            await args.Tasks.CompleteAllAsync().ConfigureAwait(false);

            args = new EngineCreateResourcesEventArgs(Engine.Current, sender, e);
            createResources.OnNext(args);
            await args.Tasks.CompleteAllAsync().ConfigureAwait(false);
        }

        internal void RegisterRenderLayer(RenderLayer renderLayer)
        {
            if (renderLayer == null)
            {
                throw new ArgumentNullException(nameof(renderLayer));
            }

            _renderLayers.Add(renderLayer);
        }

        public void Dispose()
        {
            _draw.Dispose();
            _earlyCreateResources.Dispose();
            _createResources.Dispose();
            _pointerMoved.Dispose();
        }
    }
}
