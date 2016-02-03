namespace SkiManager.Engine.ArtificialIntelligence
{
    public abstract class StrategyBase : IArtificialIntelligenceStrategy
    {

        public virtual void OnUpdate(EngineUpdateEventArgs args, IArtificialIntelligence ai, IArtificialIntelligenceState currentState) { }

        public virtual void OnParentChanged(ParentChangedEngineEventArgs args, IArtificialIntelligence ai, IArtificialIntelligenceState currentState) { }
    }
}