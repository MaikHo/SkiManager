using System;
using System.Reactive.Linq;

namespace SkiManager.Engine.Interfaces
{
    public interface IRenderer
    {
        bool IsVisible { get; set; }

        RenderLayer RenderLayer { get; set; }
    }

    public static class RendererExtensions
    {
        public static IObservable<EngineDrawEventArgs> WhereLayerIsCorrect(this IObservable<EngineDrawEventArgs> observable, IRenderer renderer)
            => observable.Where(args => args.RenderLayer == renderer.RenderLayer);

        public static IObservable<EngineDrawEventArgs> WhereShouldRender(this IObservable<EngineDrawEventArgs> observable, IRenderer renderer)
            => observable.Where(args => renderer.IsVisible).WhereLayerIsCorrect(renderer);
    }
}
