using System;
using System.Numerics;
using System.Reactive.Subjects;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace SkiManager.Engine.Behaviors
{
    [RequiresBehavior(typeof(TransformBehavior))]
    public sealed class ShapeColliderBehavior : ReactiveBehavior
    {
        private readonly Subject<CollisionEventArgs> _collisionEventSubject = new Subject<CollisionEventArgs>();
        private IDisposable _subscription;

        public IObservable<CollisionEventArgs> Collision => _collisionEventSubject;

        public SimpleGeometry Shape { get; set; }

        public Size Size { get; set; }

        public ColliderType ColliderTypes { get; set; }

        protected internal override void Loaded()
        {
            _subscription = PointerMoved.Subscribe(OnPointerMoved);
        }

        protected internal override void Destroyed()
        {
            _collisionEventSubject?.Dispose();
        }

        private void OnPointerMoved(EnginePointerMovedEventArgs args)
        {
            if (!ColliderTypes.HasFlag(ColliderType.Pointer))
            {
                return;
            }

            var mousePoint = args.Arguments.GetCurrentPoint(args.Sender as UIElement).Position.ToVector2();
            var point = Entity.GetBehavior<TransformBehavior>().GetAbsolutePosition();
            var collides = false;
            switch (Shape)
            {
                case SimpleGeometry.Circle:
                    collides = (point - mousePoint).Length() <= Size.Width / 2;
                    break;
                case SimpleGeometry.Square:
                    var minX = point.X - (Size.Width / 2);
                    var maxX = point.X + (Size.Width / 2);
                    var minY = point.Y - (Size.Height / 2);
                    var maxY = point.Y - (Size.Height / 2);
                    collides = mousePoint.X >= minX && mousePoint.X <= maxX && mousePoint.Y >= minY &&
                               mousePoint.Y <= maxY;
                    break;
            }
            if (collides)
            {
                _collisionEventSubject.OnNext(new CollisionEventArgs(Engine.Current, ColliderType.Pointer, mousePoint));
            }
        }
    }

    [Flags]
    public enum ColliderType
    {
        Pointer = 1
    }
}
