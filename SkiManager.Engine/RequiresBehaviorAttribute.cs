using System;

namespace SkiManager.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiresBehaviorAttribute : Attribute
    {
        public Type BehaviorType { get; }

        public RequiresBehaviorAttribute(Type behaviorType)
        {
            BehaviorType = behaviorType;
        }
    }
}
