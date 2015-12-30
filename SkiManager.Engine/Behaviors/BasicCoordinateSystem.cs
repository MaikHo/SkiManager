using Microsoft.Graphics.Canvas.UI.Xaml;
using SkiManager.Engine.Interfaces;
using System;
using System.Numerics;
using Windows.Foundation;

namespace SkiManager.Engine.Behaviors
{
    /// <summary>
    /// A simple coordinate system that does a 1:1-mapping of
    /// canvas space (DIPs) to world space.
    /// </summary>
    public class BasicCoordinateSystem : ReactiveBehavior, ICoordinateSystem
    {
        private IDisposable _createResourcesSubscription;
        private CanvasVirtualControl _canvasControl;

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
        }

        public Vector2 Size => _canvasControl.Size.ToVector2();

        public Vector3 Transform2DTo3D(Vector2 worldPosition)
            => new Vector3(worldPosition.X, 0, worldPosition.Y);

        public Rect TransformToDips(Rect worldRect)
            => worldRect;

        public Vector2 TransformToDips(Vector3 worldPosition)
            => worldPosition.XZ();

        public Vector2 TransformToDips(Vector2 worldPosition)
            => worldPosition;

        public Rect TransformToWorld2D(Rect dipsRect)
            => dipsRect;

        public Vector2 TransformToWorld2D(Vector2 dipsPosition)
            => dipsPosition;

        public Vector3 TransformToWorld3D(Vector2 dipsPosition)
            => new Vector3(dipsPosition.X, 0, dipsPosition.Y);
    }
}
