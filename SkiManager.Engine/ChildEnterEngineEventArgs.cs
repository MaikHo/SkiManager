namespace SkiManager.Engine
{
    public sealed class ChildEnterEngineEventArgs : EngineEventArgs
    {
        public Entity EnteringChild { get; }
        public Entity OldParent { get; }

        public ChildEnterEngineEventArgs(Engine engine, Entity enteringChild, Entity oldParent) : base(engine)
        {
            EnteringChild = enteringChild;
            OldParent = oldParent;
        }
    }
}
