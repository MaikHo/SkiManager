using System.Collections.Generic;
using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IGraphNode : ILocation
    {
        IList<Entity> AdjacentEdges { get; }
    }
}
