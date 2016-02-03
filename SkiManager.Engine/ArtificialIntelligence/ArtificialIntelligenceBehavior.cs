using System;
using System.Collections.Generic;
using System.Linq;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public class ArtificialIntelligenceBehavior : ReactiveBehavior, IArtificialIntelligence
    {
        public IArtificialIntelligenceState CurrentState { get; set; }

        public IArtificialIntelligenceStrategy CurrentStrategy { get; set; }

        public IList<IArtificialIntelligenceTransition> Transitions { get; } =
            new List<IArtificialIntelligenceTransition>();


        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Update.Subscribe(OnUpdate));
            args.TrackSubscription(ParentChanged.Subscribe(OnParentChanged));
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            if (TryTransition())
            {
                return;
            }

            CurrentStrategy?.OnUpdate(args, this, CurrentState);
        }

        private void OnParentChanged(ParentChangedEngineEventArgs args)
        {
            if (TryTransition())
            {
                return;
            }

            CurrentStrategy?.OnParentChanged(args, this, CurrentState);
        }

        private bool TryTransition()
        {
            var validTransitions = Transitions.Where(ShouldTransition).ToList();
            if (validTransitions.Count > 1)
            {
                throw new AmbiguousTransitionException(validTransitions.AsReadOnly());
            }

            if (validTransitions.Count != 1)
            {
                return false;
            }

            var transition = validTransitions.Single();
            transition.Transition(this, CurrentState);
            return true;
        }

        private bool ShouldTransition(IArtificialIntelligenceTransition transition)
            => transition.Triggers.Any(t => t.ShouldTransition(this, CurrentState));
    }
}