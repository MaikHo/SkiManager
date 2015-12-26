namespace SkiManager.Engine
{
    public sealed class ParentChangedEngineEventArgs : EngineEventArgs
    {
        public Entity OldParent { get; }
        public Entity NewParent { get; }

        public ParentChangedEngineEventArgs(Engine engine, Entity oldParent, Entity newParent) : base(engine)
        {
            OldParent = oldParent;
            NewParent = newParent;
        }
    }
}
