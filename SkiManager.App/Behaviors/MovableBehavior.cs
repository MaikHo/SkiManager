using System;
using System.Linq;
using System.Numerics;
using System.Reactive.Subjects;
using Windows.UI;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App.Behaviors
{
    public class MovableBehavior : ReactiveBehavior, IMovable
    {
        private IDisposable _subscription;
        private bool _hasTargetReached;
        private Entity _lastTarget;
        private Subject<TargetReachedEngineEventArgs> _targetReached = new Subject<TargetReachedEngineEventArgs>();

        public ILocation Location { get; private set; }

        public Entity Target { get; private set; }

        public float Speed { get; set; } = 100.0f;

        public IObservable<TargetReachedEngineEventArgs> TargetReached => _targetReached;

        public void SetTarget(Entity entity)
        {
            if (!entity.Implements<ILocation>())
            {
                throw new InvalidOperationException("Target can only be set to ILocations.");
            }
            _lastTarget = Target;
            Target = entity;
        }

        protected override void Loaded()
        {
            _subscription = Update.Subscribe(OnUpdate);
            // TODO remove debug code
            Draw.Subscribe(args =>
            {
                args.Arguments.DrawingSession.DrawText("Location: " + ((Location as ReactiveBehavior)?.Entity.Name ?? "<none>") + ", Last: " + (_lastTarget?.Name ?? "<none>"),
                    Entity.GetBehavior<TransformBehavior>().GetAbsolutePosition() + new Vector2(0, -20), Colors.DarkGray);
            });
        }

        protected override void Unloading()
        {
            _subscription?.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            if (Target == null)
            {
                return;
            }

            var thisPosition = Entity.GetBehavior<TransformBehavior>().GetAbsolutePosition();
            var targetPosition = Target?.GetBehavior<TransformBehavior>()?.GetAbsolutePosition() ?? Vector2.Zero;
            if (Vector2.Distance(thisPosition, targetPosition) <= float.Epsilon)
            {
                if (!_hasTargetReached)
                {
                    _hasTargetReached = true;
                    Location = Target?.GetImplementation<ILocation>();
                    _targetReached.OnNext(new TargetReachedEngineEventArgs(Engine.Engine.Current, Target));
                }
            }
            else
            {
                if (_hasTargetReached)
                {
                    _hasTargetReached = false;
                    Location =
                        _lastTarget?.GetImplementation<IGraphNode>()?
                            .AdjacentEdges.FirstOrDefault(
                                _ =>
                                    (_.GetImplementation<IGraphEdge>().Start == _lastTarget && _.GetImplementation<IGraphEdge>().End == Target)
                                    || (_.GetImplementation<IGraphEdge>().End == _lastTarget && _.GetImplementation<IGraphEdge>().Start == Target))
                                        ?.GetImplementation<ILocation>();
                }
                var movementVector = Vector2.Normalize(targetPosition - thisPosition);
                var movementFactor = Speed * (float)args.Arguments.Timing.ElapsedTime.TotalSeconds;
                var maxMovementFactor = Vector2.Distance(thisPosition, targetPosition);
                var newPosition = thisPosition + Math.Min(movementFactor, maxMovementFactor) * movementVector;
                Entity.GetBehavior<TransformBehavior>().Position = newPosition;
            }
        }
    }
}
