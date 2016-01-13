using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class TargetReachedEngineEventArgs : EngineEventArgs
    {
        public Entity ReachedTarget { get; }

        public SetParentResult ParentEnterResult { get; }

        public TargetReachedEngineEventArgs(Engine.Engine engine, Entity reachedTarget, SetParentResult parentEnterResult) : base(engine)
        {
            ReachedTarget = reachedTarget;
            ParentEnterResult = parentEnterResult;
        }
    }
}
