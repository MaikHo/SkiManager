using System.Collections.Generic;
using SkiManager.Engine;

namespace SkiManager.App.Interfaces
{
    public interface IGraphNode : ILocation
    {
        Entity Entity { get; }

        IList<Entity> AdjacentEdges { get; }
    }
}
