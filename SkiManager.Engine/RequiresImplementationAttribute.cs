using System;

namespace SkiManager.Engine
{
    /// <summary>
    /// Indicates that the annotated behavior requires an implementation of the provided type to be present in the behaviors of the <see cref="Entity"/> it is attached to.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class RequiresImplementationAttribute : Attribute
    {
        public Type InterfaceType { get; }

        public RequiresImplementationAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
