using System.Numerics;

namespace SkiManager.Engine.Features
{
    public sealed class MouseLeaveEngineEventArgs : MouseInteractionEngineEventArgs
    {
        public MouseLeaveEngineEventArgs(Engine engine, Vector3 lastMousePosition) : base(engine, lastMousePosition)
        {

        }
    }
}