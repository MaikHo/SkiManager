namespace SkiManager.Engine
{
    public sealed class ChildEnterEngineEventArgs : EngineEventArgs
    {
        public Entity EnteringChild { get; }
        public Entity OldParent { get; }
        public Reason Reason { get; set; }

        public ChildEnterEngineEventArgs(Engine engine, Entity enteringChild, Entity oldParent, Reason reason = default(Reason)) : base(engine)
        {
            EnteringChild = enteringChild;
            OldParent = oldParent;
            Reason = reason == default(Reason) ? Reason.None : reason;
        }
    }
}
