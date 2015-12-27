using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using SkiManager.Engine.Interfaces;
using SkiManager.Engine.Sprites;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace SkiManager.Engine.Behaviors
{
    [RequiresBehavior(typeof(TerrainBehavior))]
    public class TerrainRendererBehavior : ReactiveBehavior
    {
        private IDisposable _drawSubscription;
        private IDisposable _createResourcesSubscription;
        private PixelShaderEffect _terrainMap;
        private TerrainBehavior _terrain;

        /// <summary>
        /// A combination of the supplied height map (A channel)
        /// and a normal map (R, G, B channels).
        /// </summary>
        private CanvasBitmap _normalHeightMap;

        public SpriteReference GrassSprite { get; set; }
        public SpriteReference SnowSprite { get; set; }
        public SpriteReference RockSprite { get; set; }

        protected override void Loaded()
        {
            _drawSubscription = Draw.Subscribe(OnDraw);
            _createResourcesSubscription = CreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e)));

            _terrain = Entity.GetBehavior<TerrainBehavior>();
        }

        protected override void Unloading()
        {
            _drawSubscription.Dispose();
            _createResourcesSubscription.Dispose();
        }

        private void OnDraw(EngineDrawEventArgs e)
        {
            _terrainMap.Properties["dpi"] = e.Sender.Dpi;
            _terrainMap.Properties["baseHeight"] = _terrain.BaseHeight;
            _terrainMap.Properties["height"] = _terrain.Height - _terrain.BaseHeight;

            e.DrawingSession.DrawImage(_terrainMap);
        }

        private async Task OnCreateResourcesAsync(EngineCreateResourcesEventArgs e)
        {
            var grass = GrassSprite.Resolve(Entity);
            var snow = SnowSprite.Resolve(Entity);
            var rock = RockSprite.Resolve(Entity);
            var heightMap = _terrain.HeightMap.Resolve(Entity);

            _normalHeightMap = await RenderNormalHeightMapAsync(e.Sender, heightMap.Image, _terrain.Height - _terrain.BaseHeight);

            var wrap = CanvasEdgeBehavior.Wrap;
            var grassTiled = new BorderEffect { Source = grass.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };
            var snowTiled = new BorderEffect { Source = snow.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };
            var rockTiled = new BorderEffect { Source = rock.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };

            var shaderBytes = await Utilities.ReadBytesFromEmbeddedResourceAsync("SkiManager.Engine.Shaders.TerrainShader.bin");

            _terrainMap = new PixelShaderEffect(shaderBytes)
            {
                Source1 = _normalHeightMap,
                Source2 = grassTiled,
                Source3 = snowTiled,
                Source4 = rockTiled,
                CacheOutput = true
            };

            // TODO: Delete
            // Resize the canvas control to match the size of the height map
            //var canvasControl = e.Sender as FrameworkElement;
            //canvasControl.Width = _terrain.NormalHeightMap.Size.Width;
            //canvasControl.Height = _terrain.NormalHeightMap.Size.Height;
        }

        private void DrawSprite(EngineDrawEventArgs e, SpriteReference spriteRef, Vector2 position, Vector2 scale)
        {
            var coords = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var sprite = spriteRef.Resolve(Entity);

            var worldRect = new Rect(
                position.X - (scale.X * sprite.Size.X) / 2,
                position.Y - (scale.Y * sprite.Size.Y) / 2,
                scale.X * sprite.Size.X,
                scale.Y * sprite.Size.Y);

            var dipsRect = coords.TransformToDips(worldRect);

            e.DrawingSession.DrawImage(sprite.Image, dipsRect);
        }

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
            var bytes = await Utilities.ReadBytesFromEmbeddedResourceAsync("SkiManager.Engine.Shaders.NormalMapFromHeightMapShader.bin");

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
