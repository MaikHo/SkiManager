namespace SkiManager.Engine.ArtificialIntelligence
{
    public interface IArtificialIntelligenceStrategy
    {
        void OnUpdate(EngineUpdateEventArgs args, IArtificialIntelligence ai, IArtificialIntelligenceState currentState);

        void OnParentChanged(ParentChangedEngineEventArgs args, IArtificialIntelligence ai, IArtificialIntelligenceState currentState);
    }
}