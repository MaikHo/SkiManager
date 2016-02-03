using System.Collections.Generic;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public interface IArtificialIntelligenceState
    {
        void OnEnter(StateEnterArgs args);
    }
}