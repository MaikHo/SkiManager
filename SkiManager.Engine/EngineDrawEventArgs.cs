using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public sealed class EngineDrawEventArgs : CanvasEngineEventArgs
    {
        public CanvasAnimatedDrawEventArgs Arguments { get; }

        public EngineDrawEventArgs(Engine engine, ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) : base(engine, sender)
        {
            Arguments = args;
        }
    }
}
