using System.Numerics;

namespace SkiManager.Engine
{
    public sealed class GlobalLocation : ILocation
    {
        public Vector2 Position { get; set; }

        public GlobalLocation(Vector2 position)
        {
            Position = position;
        }
    }
}
