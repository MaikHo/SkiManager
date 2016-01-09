namespace SkiManager.Engine
{
    public sealed class ChildLeaveEngineEventArgs : EngineEventArgs
    {
        public Entity LeavingChild { get; }

        public Reason Reason { get; }

        public ChildLeaveEngineEventArgs(Engine engine, Entity leavingChild, Reason reason = default(Reason)) : base(engine)
        {
            LeavingChild = leavingChild;
            Reason = reason == default(Reason) ? Reason.None : reason;
        }
    }
}
