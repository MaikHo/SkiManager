using Microsoft.Graphics.Canvas;
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

        public Sprite(string id, Uri source, Vector2 size)
        {
            Id = id;
            Source = source;
            Size = size;
        }

        internal async Task LoadAsync(CanvasVirtualControl resourceCreator, ICoordinateSystem coords)
        {
            var canvasSizeInDips = resourceCreator.Size.ToVector2();

            Image = await CanvasBitmap.LoadAsync(resourceCreator, Source);
            var spriteSizeInPixels = new Vector2(Image.SizeInPixels.Width, Image.SizeInPixels.Height);

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
            }
            else
            {
                // TODO: Fallback to ScaleEffect
                throw new NotImplementedException();
            }

        }
    }
}
