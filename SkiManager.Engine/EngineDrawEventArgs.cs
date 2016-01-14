using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public sealed class EngineDrawEventArgs : CanvasEngineEventArgs
    {
        public CanvasRegionsInvalidatedEventArgs Arguments { get; }

        public CanvasDrawingSession DrawingSession { get; internal set; }

        public RenderLayer RenderLayer { get; }

        public EngineDrawEventArgs(Engine engine, CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args, RenderLayer renderLayer) : base(engine, sender)
        {
            Arguments = args;
            RenderLayer = renderLayer;
        }
    }
}
