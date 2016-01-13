using System;

namespace SkiManager.Engine.Behaviors
{
    public sealed class MissingBehaviorException : Exception
    {
        public Type BehaviorType { get; }

        public MissingBehaviorException(Type behaviorType) : base($"Missing required behavior {behaviorType.FullName}.")
        {
            BehaviorType = behaviorType;
        }
    }
}
