using System;
using System.Numerics;

using Windows.UI;

using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Behaviors
{
    public sealed class LineRendererBehavior : ReactiveBehavior, IRenderer
    {
        public Func<Entity, Vector2> StartPointSelector { get; set; }

        public Func<Entity, Vector2> EndPointSelector { get; set; }

        public Color Color { get; set; } = Colors.Black;

        public LineRendererBehavior() : this(null, null)
        { }

        public LineRendererBehavior(Func<Entity, Vector2> startPointSelector, Func<Entity, Vector2> endPointSelector)
        {
            StartPointSelector = startPointSelector;
            EndPointSelector = endPointSelector;
        }

        protected override void Loaded()
        {
            Draw.Subscribe(OnRender);
        }

        private void OnRender(EngineDrawEventArgs args)
        {
            if (StartPointSelector == null || EndPointSelector == null)
            {
                return;
            }

            args.Arguments.DrawingSession.DrawLine(StartPointSelector(this.Entity), EndPointSelector(this.Entity), Color);
        }
    }
}
