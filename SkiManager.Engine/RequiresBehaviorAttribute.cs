using System;

namespace SkiManager.Engine
{
    /// <summary>
    /// Indicates that the annotated behavior requires a behavior of the provided type to be present in the behaviors of the <see cref="Entity"/> it is attached to.
    /// </summary>
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
