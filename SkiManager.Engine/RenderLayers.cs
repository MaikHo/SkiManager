using System.Collections.Generic;

// ReSharper disable StaticMemberInitializerReferesToMemberBelow
namespace SkiManager.Engine
{
    public static class RenderLayers
    {
        public static IEnumerable<RenderLayer> GetAllLayers()
        {
            yield return TopMost;
            yield return Overlay;
            yield return Default;
            yield return Terrain;
            yield return BottomMost;
        }

        public static RenderLayer TopMost { get; } = new RenderLayer(int.MaxValue);

        public static RenderLayer Overlay { get; } = new RenderLayer(3000);

        public static RenderLayer Default => RenderLayer.Default;

        public static RenderLayer Terrain { get; } = new RenderLayer(-3000);

        public static RenderLayer BottomMost { get; } = new RenderLayer(int.MinValue);
    }
}
