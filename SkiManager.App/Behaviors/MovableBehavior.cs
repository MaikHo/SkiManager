using System;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.UI;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App.Behaviors
{
    [RequiresBehavior(typeof(TransformBehavior))]
    public class MovableBehavior : ReactiveBehavior, IMovable
    {
        private bool _hasTargetReached;
        private Entity _lastTarget;
        private readonly Subject<TargetReachedEngineEventArgs> _targetReached = new Subject<TargetReachedEngineEventArgs>();
        private object _lock = new object();

        public Entity Target { get; private set; }

        public float Speed { get; set; } = 10.0f;

        public IObservable<TargetReachedEngineEventArgs> TargetReached => _targetReached.AsObservable();

        public void SetTarget(Entity entity)
        {
            if (entity == null)
            {
                Target = null;
                return;
            }

            if (!entity.Implements<ILocation>())
            {
                throw new InvalidOperationException("Target can only be set to ILocations.");
            }
            _lastTarget = Target;
            Target = entity;
        }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Update.Where(_ => Target != null).Subscribe(OnUpdate));
            // TODO remove debug code
            Draw.Subscribe(arguments =>
            {
                arguments.DrawingSession.DrawText(Entity?.Name + "[" + Entity?.Id + "], Loc: " + (Entity?.Parent?.Name ?? "<none>") + ", Last: " + (_lastTarget?.Name ?? "<none>"),
                    Entity.GetBehavior<TransformBehavior>().Position.XZ() + new Vector2(0, -20), Colors.DarkGray);
            });
        }

        protected override void Destroyed()
        {
            _targetReached.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            var thisPosition = Entity.GetBehavior<TransformBehavior>().Position;
            var targetPosition = Target?.GetBehavior<TransformBehavior>()?.Position ?? Vector3.Zero;
            if (Vector3.Distance(thisPosition, targetPosition) <= float.Epsilon)
            {
                lock (_lock)
                {
                    if (!_hasTargetReached)
                    {
                        _hasTargetReached = true;
                        Entity.SetParent(Target, Reasons.TargetReached);
                        _targetReached.OnNext(new TargetReachedEngineEventArgs(Engine.Engine.Current, Target));
                    }
                }
            }
            else
            {
                if (_hasTargetReached)
                {
                    _hasTargetReached = false;
                    var location =
                        _lastTarget?.GetImplementation<IGraphNode>()?
                            .AdjacentEdges.FirstOrDefault(
                                _ =>
                                    (_.GetImplementation<IGraphEdge>().Start == _lastTarget && _.GetImplementation<IGraphEdge>().End == Target)
                                    || (_.GetImplementation<IGraphEdge>().End == _lastTarget && _.GetImplementation<IGraphEdge>().Start == Target));
                    Entity.SetParent(location, Reasons.MovingStarted);
                }
                var movementVector = Vector3.Normalize(targetPosition - thisPosition);
                var movementFactor = Speed * 0.1f; // TODO add correct timing from eventargs (this assumes 10 updates per second)
                var maxMovementFactor = Vector3.Distance(thisPosition, targetPosition);
                var newPosition = thisPosition + Math.Min(movementFactor, maxMovementFactor) * movementVector;
                Entity.GetBehavior<TransformBehavior>().Position = newPosition;
            }
        }

        public void SetLastTarget(Entity lastTarget)
        {
            _lastTarget = lastTarget;
            _hasTargetReached = true;
        }
    }
}
