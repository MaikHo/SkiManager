using System.Collections.Generic;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public interface IArtificialIntelligenceTransition
    {
        IEnumerable<IArtificialIntelligenceTransitionTrigger> Triggers { get; }

        void Transition(IArtificialIntelligence ai, IArtificialIntelligenceState currentState);
    }
}