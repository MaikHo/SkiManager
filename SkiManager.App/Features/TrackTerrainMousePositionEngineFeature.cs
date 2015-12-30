using Microsoft.Graphics.Canvas.Text;
using SkiManager.Engine;
using SkiManager.Engine.Features;
using SkiManager.Engine.Interfaces;
using System.Numerics;
using Windows.UI;
using Windows.UI.Text;

namespace SkiManager.App.Features
{
    class TrackTerrainMousePositionEngineFeature : TrackMousePositionEngineFeature
    {
        private ICoordinateSystem _coords;

        public TrackTerrainMousePositionEngineFeature(ICoordinateSystem coords, bool highlightMousePosition = false) : base(highlightMousePosition)
        {
            _coords = coords;
        }

        protected override void Draw(EngineDrawEventArgs e)
        {
            var worldPos = _coords.TransformToWorld3D(LastMouseScreenPosition);

            e.DrawingSession.DrawCircle(LastMouseScreenPosition, 4, Colors.Red);

            e.DrawingSession.DrawText(
                $"[{(int)worldPos.X}, {(int)worldPos.Y}, {(int)worldPos.Z}]",
                new Vector2(10, 10),
                Colors.Red,
                new CanvasTextFormat { FontWeight = FontWeights.Bold });
        }
    }
}
