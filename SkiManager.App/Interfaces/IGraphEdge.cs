using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IGraphEdge : ILocation
    {
        Entity Start { get; }

        Entity End { get; }

        bool IsBidirectional { get; }

        float Length { get; }
    }
}
