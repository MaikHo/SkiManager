using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using SkiManager.Engine.Interfaces;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;

namespace SkiManager.Engine.Behaviors
{
    [RequiresBehavior(typeof(TerrainBehavior))]
    public class TerrainRendererBehavior : ReactiveBehavior
    {
        private IDisposable _drawSubscription;
        private IDisposable _createResourcesSubscription;
        private PixelShaderEffect _terrainMap;

        public SpriteReference GrassSprite { get; set; }
        public SpriteReference SnowSprite { get; set; }
        public SpriteReference RockSprite { get; set; }

        protected internal override void Loaded()
        {
            _drawSubscription = Draw.Subscribe(OnDraw);
            _createResourcesSubscription = CreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e)));
        }

        protected internal override void Unloading()
        {
            _drawSubscription.Dispose();
            _createResourcesSubscription.Dispose();
        }

        private void OnDraw(EngineDrawEventArgs e)
        {
            e.Arguments.DrawingSession.DrawImage(_terrainMap);
        }

        private async Task OnCreateResourcesAsync(EngineCreateResourcesEventArgs e)
        {
            var spriteManager = Engine.Current.CurrentLevel.RootEntity.GetBehavior<SpriteManagerBehavior>();
            var grass = spriteManager.Sprites[GrassSprite];
            var snow = spriteManager.Sprites[SnowSprite];
            var rock = spriteManager.Sprites[RockSprite];

            var wrap = CanvasEdgeBehavior.Wrap;
            var grassTiled = new BorderEffect { Source = grass.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };
            var snowTiled = new BorderEffect { Source = snow.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };
            var rockTiled = new BorderEffect { Source = rock.Image, ExtendX = wrap, ExtendY = wrap, CacheOutput = true };

            var shaderBytes = await Utilities.ReadBytesFromEmbeddedResourceAsync("SkiManager.Engine.Shaders.TerrainShader.bin");

            _terrainMap = new PixelShaderEffect(shaderBytes)
            {
                Source1 = Entity.GetBehavior<TerrainBehavior>().NormalHeightMap,
                Source2 = grassTiled,
                Source3 = snowTiled,
                Source4 = rockTiled,
                CacheOutput = true
            };
        }

        private void DrawSprite(EngineDrawEventArgs e, SpriteReference spriteRef, Vector2 position, Vector2 scale)
        {
            var spriteManager = Engine.Current.CurrentLevel.RootEntity.GetBehavior<SpriteManagerBehavior>();
            var coords = Engine.Current.CurrentLevel.RootEntity.GetImplementation<ICoordinateSystem>();

            var sprite = spriteManager.Sprites[spriteRef];

            var worldRect = new Rect(
                position.X - (scale.X * sprite.Size.X) / 2,
                position.Y - (scale.Y * sprite.Size.Y) / 2,
                scale.X * sprite.Size.X,
                scale.Y * sprite.Size.Y);

            var dipsRect = coords.TransformToDips(worldRect);

            e.Arguments.DrawingSession.DrawImage(sprite.Image, dipsRect);
        }
    }
}
