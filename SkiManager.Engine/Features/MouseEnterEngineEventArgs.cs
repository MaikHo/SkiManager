using System.Numerics;

namespace SkiManager.Engine.Features
{
    public sealed class MouseEnterEngineEventArgs : MouseInteractionEngineEventArgs
    {
        public MouseEnterEngineEventArgs(Engine engine, Vector3 lastMousePosition) : base(engine, lastMousePosition)
        {

        }
    }
}