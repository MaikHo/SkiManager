using SkiManager.Engine;
using SkiManager.Engine.Interfaces;

namespace SkiManager.App.Interfaces
{
    public interface IMovable : IHasEntity
    {
        Entity Target { get; }
    }
}
