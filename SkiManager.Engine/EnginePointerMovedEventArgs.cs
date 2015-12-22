using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace SkiManager.Engine
{
    public sealed class EnginePointerMovedEventArgs : CanvasEngineEventArgs
    {
        public PointerRoutedEventArgs Arguments { get; }

        public EnginePointerMovedEventArgs(Engine engine, ICanvasAnimatedControl sender, PointerRoutedEventArgs args)
            : base(engine, sender)
        {
            Arguments = args;
        }
    }
}
