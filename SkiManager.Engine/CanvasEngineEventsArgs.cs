using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public abstract class CanvasEngineEventArgs : EngineEventArgs
    {
        public ICanvasAnimatedControl Sender { get; }

        protected CanvasEngineEventArgs(Engine engine, ICanvasAnimatedControl sender)
            : base(engine)
        {
            Sender = sender;
        }
    }
}
