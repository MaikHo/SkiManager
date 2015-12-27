using System;

namespace SkiManager.Engine
{
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
