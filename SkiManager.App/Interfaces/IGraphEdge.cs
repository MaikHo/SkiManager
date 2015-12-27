using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IGraphEdge : ILocation
    {
        Entity Entity { get; }

        Entity Start { get; }

        Entity End { get; }

        bool IsBidirectional { get; }

        float Length { get; }
    }
}
