using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public class EngineCreateResourcesEventArgs : CanvasEngineEventArgs
    {
        public TaskCollection Tasks { get; } = new TaskCollection();

        public CanvasCreateResourcesEventArgs Arguments { get; }

        public EngineCreateResourcesEventArgs(Engine engine, CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args) : base(engine, sender)
        {
            Arguments = args;
        }
    }
}
