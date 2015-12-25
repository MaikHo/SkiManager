using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class TargetReachedEngineEventArgs : EngineEventArgs
    {
        public Entity ReachedTarget { get; }

        public TargetReachedEngineEventArgs(Engine.Engine engine, Entity reachedTarget) : base(engine)
        {
            ReachedTarget = reachedTarget;
        }
    }
}
