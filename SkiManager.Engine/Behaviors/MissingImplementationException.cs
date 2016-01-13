using System;

namespace SkiManager.Engine.Behaviors
{
    public sealed class MissingImplementationException : Exception
    {
        public Type InterfaceType { get; }

        public MissingImplementationException(Type interfaceType) : base($"Missing required implementation {interfaceType.FullName}.")
        {
            InterfaceType = interfaceType;
        }
    }
}
