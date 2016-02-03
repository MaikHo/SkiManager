using System.Collections.Generic;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public abstract class TransitionBase : IArtificialIntelligenceTransition
    {
        public IEnumerable<IArtificialIntelligenceTransitionTrigger> Triggers => _triggers;

        public string Name { get; }

        private readonly IList<IArtificialIntelligenceTransitionTrigger> _triggers;

        protected TransitionBase(IEnumerable<IArtificialIntelligenceTransitionTrigger> triggers) : this(triggers, "Anonymous Transition") { }

        protected TransitionBase(IEnumerable<IArtificialIntelligenceTransitionTrigger> triggers, string name)
        {
            Name = name;
            _triggers = new List<IArtificialIntelligenceTransitionTrigger>(triggers);
        }

        public virtual void Transition(IArtificialIntelligence ai, IArtificialIntelligenceState currentState) { }
    }
}