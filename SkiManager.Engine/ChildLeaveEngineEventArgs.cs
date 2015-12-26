namespace SkiManager.Engine
{
    public sealed class ChildLeaveEngineEventArgs : EngineEventArgs
    {
        public Entity LeavingChild { get; }

        public ChildLeaveEngineEventArgs(Engine engine, Entity leavingChild) : base(engine)
        {
            LeavingChild = leavingChild;
        }
    }
}
