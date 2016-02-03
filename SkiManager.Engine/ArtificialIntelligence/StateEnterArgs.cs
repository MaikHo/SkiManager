namespace SkiManager.Engine.ArtificialIntelligence
{
    public class StateEnterArgs
    {
        public IArtificialIntelligence AI { get; }

        public Reason Reason { get; }

        public IArtificialIntelligenceState PreviousState { get; }

        public StateEnterArgs(IArtificialIntelligence ai, Reason reason, IArtificialIntelligenceState previousState)
        {
            AI = ai;
            Reason = reason;
            PreviousState = previousState;
        }
    }
}