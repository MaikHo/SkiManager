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
        private Vector2 _worldToPixels => new Vector2(
            (float)_canvasControl.Size.Width / _heightMap.Size.X,
            (float)_canvasControl.Size.Height / _heightMap.Size.Y);

        private Vector2 _pixelsToWorld => new Vector2(
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

        public float SampleHeight(Vector2 worldPosition)
        {
            var pixelPos = TransformToDips(worldPosition);

            var minX = (int)Math.Floor(pixelPos.X);
            var minY = (int)Math.Floor(pixelPos.Y);

            var tx = Mathf.InverseLerp(minX, minX + 1, pixelPos.X);
            var ty = Mathf.InverseLerp(minY, minY + 1, pixelPos.Y);

            var colors = _heightMap.Image.GetPixelColors(minX, minY, 2, 2);

            var height1 = Mathf.Lerp(ByteToWorldHeight(colors[0].A), ByteToWorldHeight(colors[1].A), tx);
            var height2 = Mathf.Lerp(ByteToWorldHeight(colors[2].A), ByteToWorldHeight(colors[3].A), tx);

            var heightResult = Mathf.Lerp(height1, height2, ty);
            return heightResult;
        }

        public Rect TransformToDips(Rect worldRect)
            => new Rect(
                TransformToDips(worldRect.Position()).ToPoint(),
                TransformToDips(worldRect.Size()).ToSize());

        public Vector2 TransformToDips(Vector3 worldPosition)
            => TransformToDips(worldPosition.XZ());

        public Vector2 TransformToDips(Vector2 worldPosition)
            => worldPosition * _worldToPixels;

        public Rect TransformToWorld(Rect dipsRect)
            => new Rect(
                TransformToWorld(dipsRect.Position()).ToPoint(),
                TransformToWorld(dipsRect.Size()).ToSize());

        public Vector2 TransformToWorld(Vector2 dipsPosition)
            => dipsPosition * _pixelsToWorld;

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
            => BaseHeight + normalizedHeight * Height;

        private float ByteToWorldHeight(byte b)
            => NormalizedToWorldHeight(b / 255f);
    }
}
