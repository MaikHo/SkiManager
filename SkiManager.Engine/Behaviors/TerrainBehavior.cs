using SkiManager.Engine.Interfaces;
using System.Numerics;
using Windows.Foundation;
using System;
using SkiManager.Engine.Sprites;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace SkiManager.Engine.Behaviors
{
    public class TerrainBehavior : ReactiveBehavior, ICoordinateSystem
    {
        private IDisposable _createResourcesSubscription;
        private Sprite _heightMap;
        private CanvasVirtualControl _canvasControl;

        // Note that the TerrainRenderer adjusts the size of the
        // canvas control to match the height map pixel size.
        private Vector2 _worldToDips => new Vector2(
            (float)_canvasControl.Size.Width / _heightMap.Size.X,
            (float)_canvasControl.Size.Height / _heightMap.Size.Y);

        private Vector2 _worldToHeightMapPixels => new Vector2(
            _heightMap.Image.SizeInPixels.Width / _heightMap.Size.X,
            _heightMap.Image.SizeInPixels.Height / _heightMap.Size.Y);

        private Vector2 _dipsToWorld => new Vector2(
            _heightMap.Size.X / (float)_canvasControl.Size.Width,
            _heightMap.Size.Y / (float)_canvasControl.Size.Height);

        /// <summary>
        /// The grayscale height map image.
        /// </summary>
        public SpriteReference HeightMap { get; set; }

        /// <summary>
        /// The absolute height represented by white height map pixels.
        /// This means that <see cref="Height"/> - <see cref="BaseHeight"/>
        /// is the maximum height difference that can be represented by
        /// the height map.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The minimum height that can be represented.
        /// </summary>
        /// <remarks>
        /// This is the height represented by black height map pixels.
        /// </remarks>
        public float BaseHeight { get; set; }

        Vector2 ICoordinateSystem.Size => HeightMap.Resolve(Entity).Size; // Can't use _heightMap here because CreateResources might not have fired yet

        /// <summary>
        /// Gets the height in world space at the specified world position.
        /// </summary>
        /// <param name="worldPosition">2D world position (clamped to size of terrain)</param>
        /// <returns>Height</returns>
        public float SampleHeight(Vector2 worldPosition)
        {
            var pixelPos = Vector2.Clamp(worldPosition * _worldToHeightMapPixels, Vector2.Zero, new Vector2(_heightMap.Image.SizeInPixels.Width - 1, _heightMap.Image.SizeInPixels.Height - 1));

            var minX = (int)Math.Floor(pixelPos.X);
            var minY = (int)Math.Floor(pixelPos.Y);

            var tx = Mathf.InverseLerp(pixelPos.X, minX, minX + 1);
            var ty = Mathf.InverseLerp(pixelPos.Y, minY, minY + 1);

            // Calculate how many pixels we can read from heightmap
            // At the right and bottom edges we can only read a 1x1 rect of colors.
            var width = (minX == _heightMap.Image.SizeInPixels.Width - 1) ? 1 : 2;
            var height = (minY == _heightMap.Image.SizeInPixels.Height - 1) ? 1 : 2;

            var colors = _heightMap.Image.GetPixelColors(minX, minY, width, height);

            if (width == 1)
            {
                if (height == 1)
                { // 1x1 pixel rect => no interpolation
                    return ByteToWorldHeight(colors[0].R);
                }
                else
                { // 1x2 pixel rect => interpolate with ty
                    return Mathf.Lerp(ByteToWorldHeight(colors[0].R), ByteToWorldHeight(colors[1].R), ty);
                }
            }
            else
            {
                if (height == 1)
                { // 2x1 pixel rect => interpolate with tx
                    return Mathf.Lerp(ByteToWorldHeight(colors[0].R), ByteToWorldHeight(colors[1].R), tx);
                }
                else
                { // 2x2 pixels => bilinear interpolation
                    var height2 = Mathf.Lerp(ByteToWorldHeight(colors[2].R), ByteToWorldHeight(colors[3].R), tx);
                    var height1 = Mathf.Lerp(ByteToWorldHeight(colors[0].R), ByteToWorldHeight(colors[1].R), tx);
                    return Mathf.Lerp(height1, height2, ty);
                }
            }
        }

        public Rect TransformToDips(Rect worldRect)
            => new Rect(
                TransformToDips(worldRect.Position()).ToPoint(),
                TransformToDips(worldRect.Size()).ToSize());

        public Vector2 TransformToDips(Vector3 worldPosition)
            => TransformToDips(worldPosition.XZ());

        public Vector2 TransformToDips(Vector2 worldPosition)
            => worldPosition * _worldToDips;

        public Rect TransformToWorld2D(Rect dipsRect)
            => new Rect(
                TransformToWorld2D(dipsRect.Position()).ToPoint(),
                TransformToWorld2D(dipsRect.Size()).ToSize());

        public Vector3 TransformToWorld3D(Vector2 dipsPosition)
            => new Vector3(dipsPosition.X * _dipsToWorld.X, SampleHeight(dipsPosition * _dipsToWorld), dipsPosition.Y * _dipsToWorld.Y);

        public Vector2 TransformToWorld2D(Vector2 dipsPosition)
            => dipsPosition * _dipsToWorld;

        public Vector3 Transform2DTo3D(Vector2 worldPosition)
            => new Vector3(worldPosition.X, SampleHeight(worldPosition), worldPosition.Y);

        protected override void Loaded()
        {
            _createResourcesSubscription = CreateResources.Subscribe(OnCreateResources);
        }

        protected override void Unloading()
        {
            _createResourcesSubscription.Dispose();
        }

        private void OnCreateResources(EngineCreateResourcesEventArgs e)
        {
            _canvasControl = e.Sender;
            _heightMap = HeightMap.Resolve(Entity);
        }



        /// <summary>
        /// Converts a normalized height (between 0 and 1) into world height.
        /// </summary>
        /// <param name="normalizedHeight">Height in range [0..1]</param>
        /// <returns>Height in meters</returns>
        private float NormalizedToWorldHeight(float normalizedHeight)
            => Mathf.Lerp(BaseHeight, Height, normalizedHeight);

        private float ByteToWorldHeight(byte b)
            => NormalizedToWorldHeight(b / 255f);
    }
}
