using System.Numerics;

namespace SkiManager.Engine.Features
{
    public abstract class MouseInteractionEngineEventArgs : EngineEventArgs
    {
        public Vector3 LastMousePosition { get; }

        protected MouseInteractionEngineEventArgs(Engine engine, Vector3 lastMousePosition) : base(engine)
        {
            LastMousePosition = lastMousePosition;
        }
    }
}