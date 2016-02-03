using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public interface IArtificialIntelligence : IHasEntity
    {
        IArtificialIntelligenceState CurrentState { get; set; }

        IArtificialIntelligenceStrategy CurrentStrategy { get; set; }
    }
}