using System;

namespace SkiManager.App
{
    public class InvalidLevelConfigurationException : Exception
    {
        public InvalidLevelConfigurationException()
        {
        }

        public InvalidLevelConfigurationException(string message) : base(message)
        {
        }

        public InvalidLevelConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
