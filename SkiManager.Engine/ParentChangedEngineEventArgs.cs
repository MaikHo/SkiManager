namespace SkiManager.Engine
{
    public sealed class ParentChangedEngineEventArgs : EngineEventArgs
    {
        public Entity OldParent { get; }
        public Entity NewParent { get; }
        public Reason Reason { get; }

        public ParentChangedEngineEventArgs(Engine engine, Entity oldParent, Entity newParent, Reason reason = default(Reason)) : base(engine)
        {
            OldParent = oldParent;
            NewParent = newParent;
            Reason = reason == default(Reason) ? Reason.None : reason;
        }
    }
}
