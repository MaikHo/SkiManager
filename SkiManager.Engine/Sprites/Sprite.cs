using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SkiManager.Engine.Interfaces;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace SkiManager.Engine.Sprites
{
    public class Sprite
    {
        public string Id { get; }

        public Uri Source { get; }

        /// <summary>
        /// Virtual size in world coordinates.
        /// </summary>
        public Vector2 Size { get; }

        public CanvasBitmap Image { get; private set; }

        /// <summary>
        /// Indicates whether the sprite image is already
        /// scaled using its DPI property. If this is false,
        /// the renderer must handle scaling.
        /// </summary>
        /// <remarks>
        /// The sprite image can be prescaled only if the sprite
        /// and the world have the same aspect ratio regarding the
        /// virtual world size and also regarding the pixel size.
        /// </remarks>
        public bool IsPreScaled { get; private set; }

        public Sprite(string id, Uri source, Vector2 size)
        {
            Id = id;
            Source = source;
            Size = size;
        }

        internal async Task LoadAsync(CanvasVirtualControl resourceCreator, ICoordinateSystem coords)
        {
            Image = await CanvasBitmap.LoadAsync(resourceCreator, Source);
            var spriteSizeInPixels = new Vector2(Image.SizeInPixels.Width, Image.SizeInPixels.Height);
            var canvasSizeInDips = resourceCreator.Size.ToVector2();

            var pixelRatio = spriteSizeInPixels / canvasSizeInDips;
            var worldSizeRatio = Size / coords.Size;

            if (pixelRatio.X == pixelRatio.Y && worldSizeRatio.X == worldSizeRatio.Y)
            {
                // If world and sprite have the same aspect ratio we can load
                // the sprite with manipulated DPI so we do not need to scale
                // it in DrawImage(...) or using a ScaleEffect
                var dpi = (96 * pixelRatio.X) / worldSizeRatio.X;
                Image.Dispose();
                Image = await CanvasBitmap.LoadAsync(resourceCreator, Source, dpi);
                IsPreScaled = true;
            }
            else
            {
                // Otherwise, the renderer must do the scaling
                IsPreScaled = false;
            }
        }
    }
}
