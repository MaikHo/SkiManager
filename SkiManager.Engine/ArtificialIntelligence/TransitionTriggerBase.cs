namespace SkiManager.Engine.ArtificialIntelligence
{
    public abstract class TransitionTriggerBase : IArtificialIntelligenceTransitionTrigger
    {
        public virtual bool ShouldTransition(IArtificialIntelligence ai, IArtificialIntelligenceState currentState) => false;
    }
}