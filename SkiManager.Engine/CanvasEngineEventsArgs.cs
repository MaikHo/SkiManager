using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public abstract class CanvasEngineEventArgs : EngineEventArgs
    {
        public CanvasVirtualControl Sender { get; }

        protected CanvasEngineEventArgs(Engine engine, CanvasVirtualControl sender)
            : base(engine)
        {
            Sender = sender;
        }
    }
}
