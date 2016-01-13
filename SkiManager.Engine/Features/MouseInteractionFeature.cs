using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reactive.Linq;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Features
{
    public sealed class MouseInteractionFeature : EngineFeature
    {
        private readonly Dictionary<Entity, IObservable<MouseEnterEngineEventArgs>> _enterObservablesLookup = new Dictionary<Entity, IObservable<MouseEnterEngineEventArgs>>();
        private readonly Dictionary<Entity, IObservable<MouseMoveOverEngineEventArgs>> _moveOverObservablesLookup = new Dictionary<Entity, IObservable<MouseMoveOverEngineEventArgs>>();
        private readonly Dictionary<Entity, IObservable<MouseLeaveEngineEventArgs>> _leaveObservablesLookup = new Dictionary<Entity, IObservable<MouseLeaveEngineEventArgs>>();
        private readonly Dictionary<Entity, MousePointerOverEntityState> _lastEntityStates = new Dictionary<Entity, MousePointerOverEntityState>();

        private Vector3 _lastMousePosition = Vector3.Zero;
        private IDisposable _subscription;


        public static IObservable<MouseEnterEngineEventArgs> MouseEnterFor(Entity entity)
        {
            EnsureAvailabilityOfFeatureAndTransform(entity);

            var feature = Engine.Current.GetFeature<MouseInteractionFeature>();

            if (!feature._enterObservablesLookup.ContainsKey(entity))
            {
                feature._enterObservablesLookup.Add(entity, CreateMouseEnterForEntity(entity, feature));
            }
            EnsureEntryInLastMousePointerStates(entity, feature);

            return feature._enterObservablesLookup[entity].AsObservable();
        }

        public static IObservable<MouseMoveOverEngineEventArgs> MouseMoveOverFor(Entity entity)
        {
            EnsureAvailabilityOfFeatureAndTransform(entity);

            var feature = Engine.Current.GetFeature<MouseInteractionFeature>();

            if (!feature._moveOverObservablesLookup.ContainsKey(entity))
            {
                feature._moveOverObservablesLookup.Add(entity, CreateMouseMoveOverForEntity(entity, feature));
            }
            EnsureEntryInLastMousePointerStates(entity, feature);

            return feature._moveOverObservablesLookup[entity].AsObservable();
        }

        public static IObservable<MouseLeaveEngineEventArgs> MouseLeaveFor(Entity entity)
        {
            EnsureAvailabilityOfFeatureAndTransform(entity);

            var feature = Engine.Current.GetFeature<MouseInteractionFeature>();

            if (!feature._leaveObservablesLookup.ContainsKey(entity))
            {
                feature._leaveObservablesLookup.Add(entity, CreateMouseLeaveForEntity(entity, feature));
            }
            EnsureEntryInLastMousePointerStates(entity, feature);

            return feature._leaveObservablesLookup[entity].AsObservable();
        }

        private static void EnsureEntryInLastMousePointerStates(Entity entity, MouseInteractionFeature feature)
        {
            if (!feature._lastEntityStates.ContainsKey(entity))
            {
                feature._lastEntityStates.Add(entity, MousePointerOverEntityState.Unknown);
            }
        }

        protected override void Attach()
        {
            _subscription = Engine.Events.PointerMoved.Subscribe(StorePointerPosition);
        }

        protected override void Detach()
        {
            _subscription.Dispose();
        }

        private static MouseEnterEngineEventArgs CreateMouseEnterEventArgs(Entity entity, MouseInteractionFeature feature, EngineUpdateEventArgs args)
        {
            return new MouseEnterEngineEventArgs(Engine.Current, feature._lastMousePosition);
        }

        private static IObservable<MouseEnterEngineEventArgs> CreateMouseEnterForEntity(Entity entity,
            MouseInteractionFeature feature)
        {
            return Engine.Current.Events.Update.Where(args => IsMousePointerEnteringEntity(entity, feature, args))
                    .Select(args => CreateMouseEnterEventArgs(entity, feature, args))
                    .Publish()
                    .RefCount();
        }

        private static MouseMoveOverEngineEventArgs CreateMouseMoveOverEventArgs(Entity entity, MouseInteractionFeature feature, EnginePointerMovedEventArgs args)
        {
            return new MouseMoveOverEngineEventArgs(Engine.Current, feature._lastMousePosition);
        }

        private static IObservable<MouseMoveOverEngineEventArgs> CreateMouseMoveOverForEntity(Entity entity, MouseInteractionFeature feature)
        {
            return Engine.Current.Events.PointerMoved.Where(args => IsMousePointerMovingOverEntity(entity, feature, args))
                    .Select(args => CreateMouseMoveOverEventArgs(entity, feature, args))
                    .Publish()
                    .RefCount();
        }

        private static MouseLeaveEngineEventArgs CreateMouseLeaveEventArgs(Entity entity, MouseInteractionFeature feature, EngineUpdateEventArgs args)
        {
            return new MouseLeaveEngineEventArgs(Engine.Current, feature._lastMousePosition);
        }

        private static IObservable<MouseLeaveEngineEventArgs> CreateMouseLeaveForEntity(Entity entity,
            MouseInteractionFeature feature)
        {
            return Engine.Current.Events.Update.Where(args => IsMousePointerLeavingEntity(entity, feature, args))
                    .Select(args => CreateMouseLeaveEventArgs(entity, feature, args))
                    .Publish()
                    .RefCount();
        }

        private static void EnsureAvailabilityOfFeatureAndTransform(Entity entity)
        {
            if (!Engine.Current.HasFeature<MouseInteractionFeature>())
            {
                throw new MissingFeatureException(typeof(MouseInteractionFeature));
            }

            if (!entity.Implements<IReadOnlyTransform>())
            {
                throw new MissingImplementationException(typeof(IReadOnlyTransform));
            }
        }

        private static bool IsMousePointerEnteringEntity(Entity entity, MouseInteractionFeature feature, EngineUpdateEventArgs args)
        {
            var isEntering = IsLastMousePositionOverEntity(feature, entity) && feature._lastEntityStates[entity] != MousePointerOverEntityState.Over;
            if (isEntering)
            {
                feature._lastEntityStates[entity] = MousePointerOverEntityState.Over;
            }
            return isEntering;
        }

        private static bool IsMousePointerMovingOverEntity(Entity entity, MouseInteractionFeature feature, EnginePointerMovedEventArgs args)
        {
            var wasOver = feature._lastEntityStates[entity] == MousePointerOverEntityState.Over;
            return IsLastMousePositionOverEntity(feature, entity) && wasOver;
        }

        private static bool IsMousePointerLeavingEntity(Entity entity, MouseInteractionFeature feature, EngineUpdateEventArgs args)
        {
            var isLeaving = !IsLastMousePositionOverEntity(feature, entity) && feature._lastEntityStates[entity] == MousePointerOverEntityState.Over;
            if (isLeaving)
            {
                feature._lastEntityStates[entity] = MousePointerOverEntityState.NotOver;
            }
            return isLeaving;
        }

        private static bool IsLastMousePositionOverEntity(MouseInteractionFeature feature, Entity entity)
        {
            var transform = entity.GetImplementation<IReadOnlyTransform>();

            // TODO check bounding box instead of value and check for overlapping entities
            return Vector3.Distance(transform.Position, feature._lastMousePosition) < 6.0f;
        }

        private void StorePointerPosition(EnginePointerMovedEventArgs args)
        {
            var coordinateSystem = Engine.CurrentLevel.RootEntity.GetImplementation<ICoordinateSystem>(); // TODO this might be better positioned inside the engine?!
            var mousePosition = coordinateSystem.TransformToWorld3D(args.Arguments.GetCurrentPoint(args.Sender).Position.ToVector2());
            _lastMousePosition = mousePosition;
        }

        private enum MousePointerOverEntityState
        {
            Unknown,
            NotOver,
            Over
        }
    }
}
