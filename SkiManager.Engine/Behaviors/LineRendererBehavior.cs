using System;
using System.Numerics;
using System.Reactive.Linq;
using Windows.UI;

using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Behaviors
{
    public sealed class LineRendererBehavior : ReactiveBehavior, IRenderer
    {
        public Func<Entity, Vector3> StartPointSelector { get; set; }

        public Func<Entity, Vector3> EndPointSelector { get; set; }

        public Color Color { get; set; } = Colors.Black;

        public bool IsVisible { get; set; } = true;

        public LineRendererBehavior() : this(null, null)
        { }

        public LineRendererBehavior(Func<Entity, Vector3> startPointSelector, Func<Entity, Vector3> endPointSelector)
        {
            StartPointSelector = startPointSelector;
            EndPointSelector = endPointSelector;
        }

        protected override void Loaded()
        {
            Draw.Where(_ => IsVisible).Subscribe(OnRender);
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            if (StartPointSelector == null || EndPointSelector == null)
            {
                return;
            }

            var coordinateSystem = Entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
            var startPoint = coordinateSystem.TransformToDips(StartPointSelector(Entity));
            var endPoint = coordinateSystem.TransformToDips(EndPointSelector(Entity));

            args.DrawingSession.DrawLine(startPoint, endPoint, Color);
        }
    }
}
