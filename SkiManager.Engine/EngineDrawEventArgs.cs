using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public sealed class EngineDrawEventArgs : CanvasEngineEventArgs
    {
        public CanvasRegionsInvalidatedEventArgs Arguments { get; }

        public CanvasDrawingSession DrawingSession { get; internal set; }

        public EngineDrawEventArgs(Engine engine, CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args) : base(engine, sender)
        {
            Arguments = args;
        }
    }
}
