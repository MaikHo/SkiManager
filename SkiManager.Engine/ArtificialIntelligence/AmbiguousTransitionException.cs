using System;
using System.Collections.Generic;

namespace SkiManager.Engine.ArtificialIntelligence
{
    public class AmbiguousTransitionException : Exception
    {
        public IReadOnlyList<IArtificialIntelligenceTransition> ConflictingTransitions { get; }

        public AmbiguousTransitionException(IReadOnlyList<IArtificialIntelligenceTransition> conflictingTransitions) : base("Ambiguous transitions found.")
        {
            ConflictingTransitions = conflictingTransitions;
        }
    }
}