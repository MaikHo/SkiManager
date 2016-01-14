using System;

namespace SkiManager.Engine
{
    public sealed class EngineUpdateEventArgs : EngineEventArgs
    {

        public TimeSpan DeltaTime { get; }

        public TimeSpan GameTime { get; }

        public EngineUpdateEventArgs(Engine engine, TimeSpan deltaTime, TimeSpan gameTime)
            : base(engine)
        {
            DeltaTime = deltaTime;
            GameTime = gameTime;
        }
    }
}
