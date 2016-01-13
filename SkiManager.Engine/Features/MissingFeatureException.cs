using System;

namespace SkiManager.Engine.Features
{
    public sealed class MissingFeatureException : Exception
    {
        public Type FeatureType { get; }

        public MissingFeatureException(Type featureType) : base($"Missing required feature {featureType.FullName}.")
        {
            FeatureType = featureType;
        }
    }
}
