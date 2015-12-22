using System.Linq;

namespace SkiManager.Engine
{
    public static class EngineExtensions
    {
        public static bool HasFeature<TFeature>(this Engine engine) => engine.Features.OfType<TFeature>().Any();

        public static TFeature GetFeature<TFeature>(this Engine engine) => engine.Features.OfType<TFeature>().First();
    }
}
