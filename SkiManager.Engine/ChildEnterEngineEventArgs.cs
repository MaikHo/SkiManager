namespace SkiManager.Engine
{
    public sealed class ChildEnterEngineEventArgs : EngineEventArgs
    {
        public Entity EnteringChild { get; }

        public ChildEnterEngineEventArgs(Engine engine, Entity enteringChild) : base(engine)
        {
            EnteringChild = enteringChild;
        }
    }
}
