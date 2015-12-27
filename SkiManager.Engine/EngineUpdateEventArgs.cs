using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public sealed class EngineUpdateEventArgs : CanvasEngineEventArgs
    {
        public CanvasAnimatedUpdateEventArgs Arguments { get; }

        public EngineUpdateEventArgs(Engine engine, CanvasVirtualControl sender, CanvasAnimatedUpdateEventArgs args)
            : base(engine, sender)
        {
            Arguments = args;
        }
    }
}
