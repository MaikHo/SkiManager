using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IGraphEdge : ILocation
    {
        IGraphNode Start { get; }

        IGraphNode End { get; }

        bool IsBidirectional { get; }

        float Length { get; }
    }
}
