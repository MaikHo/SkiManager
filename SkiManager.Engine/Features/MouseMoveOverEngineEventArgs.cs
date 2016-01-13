using System.Numerics;

namespace SkiManager.Engine.Features
{
    public sealed class MouseMoveOverEngineEventArgs : MouseInteractionEngineEventArgs
    {
        public MouseMoveOverEngineEventArgs(Engine engine, Vector3 lastMousePosition) : base(engine, lastMousePosition)
        {

        }
    }
}