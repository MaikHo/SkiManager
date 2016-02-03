namespace SkiManager.Engine.ArtificialIntelligence
{
    public interface IArtificialIntelligenceTransitionTrigger
    {
        bool ShouldTransition(IArtificialIntelligence ai, IArtificialIntelligenceState currentState);
    }
}