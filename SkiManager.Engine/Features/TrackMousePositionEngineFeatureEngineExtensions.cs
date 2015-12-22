using System;
using System.Numerics;

namespace SkiManager.Engine.Features
{
    public static class TrackMousePositionEngineFeatureEngineExtensions
    {
        public static Vector2 GetLastMousePos(this Engine engine)
        {
            if (!engine.HasFeature<TrackMousePositionEngineFeature>())
            {
                throw new InvalidOperationException("Engine does not contain the feature.");
            }

            return engine.GetFeature<TrackMousePositionEngineFeature>().LastMouseScreenPos;
        }
    }
}