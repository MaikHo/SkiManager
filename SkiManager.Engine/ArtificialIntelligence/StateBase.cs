namespace SkiManager.Engine.ArtificialIntelligence
{
    public abstract class StateBase : IArtificialIntelligenceState
    {
        public virtual void OnEnter(StateEnterArgs args) { }
    }
}