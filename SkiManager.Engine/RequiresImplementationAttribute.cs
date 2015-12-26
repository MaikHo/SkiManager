using System;

namespace SkiManager.Engine
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    sealed class RequiresImplementationAttribute : Attribute
    {
        public Type InterfaceType { get; }

        public RequiresImplementationAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
