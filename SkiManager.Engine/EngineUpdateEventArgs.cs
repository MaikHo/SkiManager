using System;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine
{
    public sealed class EngineUpdateEventArgs : CanvasEngineEventArgs
    {
        public CanvasAnimatedUpdateEventArgs Arguments { get; }

        public TimeSpan DeltaTime { get; }

        public TimeSpan GameTime { get; }

        public EngineUpdateEventArgs(Engine engine, CanvasVirtualControl sender, CanvasAnimatedUpdateEventArgs args, TimeSpan deltaTime, TimeSpan gameTime)
            : base(engine, sender)
        {
            Arguments = args;
            DeltaTime = deltaTime;
            GameTime = gameTime;
        }
    }
}
