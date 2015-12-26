using SkiManager.Engine.Interfaces;
using System.Numerics;
using Windows.Foundation;
using System;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;

namespace SkiManager.Engine.Behaviors
{
    public class TerrainBehavior : ReactiveBehavior, ICoordinateSystem
    {
        private IDisposable _createResourcesSubscription;

        /// <summary>
        /// A combination of the supplied height map (A channel)
        /// and a normal map (R, G, B channels).
        /// </summary>
        internal CanvasBitmap NormalHeightMap { get; private set; }

        /// <summary>
        /// The grayscale height map image.
        /// </summary>
        public Uri HeightMapSource { get; set; }

        /// <summary>
        /// The size of the terrain in world coordinates.
        /// </summary>
        /// <remarks>
        /// The Y coordinate is the absolute height represented by
        /// white height map pixels. This means that
        /// Size.Y - <see cref="BaseHeight"/> is the maximum height
        /// difference that can be represented by the height map.
        /// </remarks>
        public Vector3 Size { get; set; }

        /// <summary>
        /// The minimum height that can be represented.
        /// </summary>
        /// <remarks>
        /// This is the height represented by black height map pixels.
        /// </remarks>
        public float BaseHeight { get; set; }

        public float SampleHeight(Vector2 position)
        {
            var worldToPixels = new Vector2(
                NormalHeightMap.SizeInPixels.Width / Size.X,
                NormalHeightMap.SizeInPixels.Height / Size.Z);

            var pixelPos = position * worldToPixels;

            var minX = (int)Math.Floor(pixelPos.X);
            var minY = (int)Math.Floor(pixelPos.Y);

            var tx = Mathf.InverseLerp(minX, minX + 1, pixelPos.X);
            var ty = Mathf.InverseLerp(minY, minY + 1, pixelPos.Y);

            var colors = NormalHeightMap.GetPixelColors(minX, minY, 2, 2);

            var height1 = Mathf.Lerp(ByteToWorldHeight(colors[0].A), ByteToWorldHeight(colors[1].A), tx);
            var height2 = Mathf.Lerp(ByteToWorldHeight(colors[2].A), ByteToWorldHeight(colors[3].A), tx);

            var heightResult = Mathf.Lerp(height1, height2, ty);
            return heightResult;
        }

        public Rect TransformToDips(Rect worldRect)
        {
            throw new NotImplementedException();
        }

        public Vector2 TransformToDips(Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }

        public Vector2 TransformToDips(Vector2 worldPosition)
        {
            throw new NotImplementedException();
        }

        public Rect TransformToWorld(Rect dipsRect)
        {
            throw new NotImplementedException();
        }

        public Vector3 TransformToWorld(Vector2 dipsPosition)
        {
            throw new NotImplementedException();
        }

        protected override void Loaded()
        {
            _createResourcesSubscription = CreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e)));
        }

        private async Task OnCreateResourcesAsync(EngineCreateResourcesEventArgs e)
        {
            var heightMap = await CanvasBitmap.LoadAsync(e.Sender, HeightMapSource);
            NormalHeightMap = await RenderNormalHeightMapAsync(e.Sender, heightMap, Size.Y - BaseHeight);
        }



        /// <summary>
        /// Converts a normalized height (between 0 and 1) into world height.
        /// </summary>
        /// <param name="normalizedHeight">Height in range [0..1]</param>
        /// <returns>Height in meters</returns>
        private float NormalizedToWorldHeight(float normalizedHeight)
            => BaseHeight + normalizedHeight * Size.Y;

        private float ByteToWorldHeight(byte b)
            => NormalizedToWorldHeight(b / 255f);




        /// <summary>
        /// Renders a texture that contains surface normals
        /// in the R, G and B channels and height information in
        /// the A channel based on the specified height map.
        /// </summary>
        /// <param name="resourceCreator">Resource creator</param>
        /// <param name="heightMap">Height map</param>
        /// <returns>A combination of a normal map and the height map</returns>
        private static async Task<CanvasBitmap> RenderNormalHeightMapAsync(ICanvasResourceCreatorWithDpi resourceCreator, CanvasBitmap heightMap, float heightDifference)
        {
            var bytes = await Utilities.ReadBytesAsync(new Uri("SkiManager.Engine.Shaders.NormalMapFromHeightMapShader.bin"));
            
            var heightMapConverterEffect = new PixelShaderEffect(bytes)
            {
                Source1 = heightMap,
                Source1Mapping = SamplerCoordinateMapping.Offset,
                MaxSamplerOffset = 1,
            };

            heightMapConverterEffect.Properties["dpi"] = resourceCreator.Dpi;
            heightMapConverterEffect.Properties["height"] = heightDifference;

            var normalHeightMap = new CanvasRenderTarget(
                resourceCreator,
                (float)heightMap.Size.Width,
                (float)heightMap.Size.Height,
                heightMap.Dpi);

            using (var g = normalHeightMap.CreateDrawingSession())
            {
                g.DrawImage(heightMapConverterEffect);
            }

            return normalHeightMap;
        }

    }
}
