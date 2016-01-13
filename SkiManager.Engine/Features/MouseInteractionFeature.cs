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
        private readonly Dictionary<Entity, IObservable<MouseLeaveEngineEventArgs>> _leaveObservablesLookup = new Dictionary<Entity, IObservable<MouseLeaveEngineEventArgs>>();
        private Entity _currentMouseOverEntity;
        private Vector3 _lastMousePosition = Vector3.Zero;


        protected override void Attach()
        {
            // Method intentionally left empty.
        }

        protected override void Detach()
        {
            // Method intentionally left empty.
        }

        public static IObservable<MouseEnterEngineEventArgs> MouseEnterFor(Entity entity)
        {
            if (!Engine.Current.HasFeature<MouseInteractionFeature>())
            {
                throw new MissingFeatureException(typeof(MouseInteractionFeature));
            }

            if (!entity.Implements<IReadOnlyTransform>())
            {
                throw new MissingImplementationException(typeof(IReadOnlyTransform));
            }

            var feature = Engine.Current.GetFeature<MouseInteractionFeature>();
            var subjects = feature._enterObservablesLookup;
            if (!subjects.ContainsKey(entity))
            {
                subjects.Add(entity, CreateMouseEnteredForEntity(entity, feature));
            }

            return subjects[entity].AsObservable();
        }

        private static IObservable<MouseEnterEngineEventArgs> CreateMouseEnteredForEntity(Entity entity,
            MouseInteractionFeature feature)
        {
            var pointerMovedConverted = Engine.Current.Events.PointerMoved.Select(args => new PointerOrUpdateEventArgs { PointerArgs = args });
            var updateConverted = Engine.Current.Events.Update.Select(args => new PointerOrUpdateEventArgs { UpdateArgs = args });
            var merged = pointerMovedConverted.Merge(updateConverted);
            return
                merged.Where(args => IsMousePointerEnteringEntity(entity, feature, args))
                    .Select(args => CreateMouseEnterEventArgs(entity, feature, args))
                    .Publish()
                    .RefCount();
        }

        private static MouseEnterEngineEventArgs CreateMouseEnterEventArgs(Entity entity, MouseInteractionFeature feature, PointerOrUpdateEventArgs args) => new MouseEnterEngineEventArgs(Engine.Current);

        public static IObservable<MouseLeaveEngineEventArgs> MouseLeaveFor(Entity entity)
        {
            if (!Engine.Current.HasFeature<MouseInteractionFeature>())
            {
                throw new MissingFeatureException(typeof(MouseInteractionFeature));
            }

            if (!entity.Implements<IReadOnlyTransform>())
            {
                throw new MissingImplementationException(typeof(IReadOnlyTransform));
            }

            var feature = Engine.Current.GetFeature<MouseInteractionFeature>();
            var subjects = feature._leaveObservablesLookup;
            if (!subjects.ContainsKey(entity))
            {
                subjects.Add(entity, CreateMouseLeaveForEntity(entity, feature));
            }

            return subjects[entity].AsObservable();
        }

        private static IObservable<MouseLeaveEngineEventArgs> CreateMouseLeaveForEntity(Entity entity,
            MouseInteractionFeature feature)
        {
            var pointerMovedConverted = Engine.Current.Events.PointerMoved.Select(args => new PointerOrUpdateEventArgs { PointerArgs = args });
            var updateConverted = Engine.Current.Events.Update.Select(args => new PointerOrUpdateEventArgs { UpdateArgs = args });
            var merged = pointerMovedConverted.Merge(updateConverted);
            return
                merged.Where(args => IsMousePointerLeavingEntity(entity, feature, args))
                    .Select(args => CreateMouseLeaveEventArgs(entity, feature, args))
                    .Publish()
                    .RefCount();
        }

        private static MouseLeaveEngineEventArgs CreateMouseLeaveEventArgs(Entity entity, MouseInteractionFeature feature, PointerOrUpdateEventArgs args) => new MouseLeaveEngineEventArgs(Engine.Current);

        private static bool IsMousePointerEnteringEntity(Entity entity, MouseInteractionFeature feature, PointerOrUpdateEventArgs args)
        {
            if (args.PointerArgs != null)
            {
                var coordinateSystem = entity.Level.RootEntity.GetImplementation<ICoordinateSystem>(); // TODO this might be better positioned inside the engine?!
                var mousePosition = coordinateSystem.TransformToWorld3D(args.PointerArgs.Arguments.GetCurrentPoint(args.PointerArgs.Sender).Position.ToVector2());
                feature._lastMousePosition = mousePosition;
            }

            var isEntering = IsOverEntity(feature, entity) && !Equals(feature._currentMouseOverEntity, entity);
            feature._currentMouseOverEntity = entity;
            return isEntering;
        }

        private static bool IsMousePointerLeavingEntity(Entity entity, MouseInteractionFeature feature,
            PointerOrUpdateEventArgs args)
        {
            if (args.PointerArgs != null) // TODO refactor to only do this once
            {
                var coordinateSystem = entity.Level.RootEntity.GetImplementation<ICoordinateSystem>();
                var mousePosition = coordinateSystem.TransformToWorld3D(args.PointerArgs.Arguments.GetCurrentPoint(args.PointerArgs.Sender).Position.ToVector2());
                feature._lastMousePosition = mousePosition;
            }

            var isLeaving = !IsOverEntity(feature, entity) && Equals(feature._currentMouseOverEntity, entity);
            feature._currentMouseOverEntity = null;
            return isLeaving;
        }

        private static bool IsOverEntity(MouseInteractionFeature feature, Entity entity)
        {
            var transform = entity.GetImplementation<IReadOnlyTransform>();

            // TODO check bounding box instead of value and check for overlapping entities
            return Vector3.Distance(transform.Position, feature._lastMousePosition) < 6.0f;
        }

        private class PointerOrUpdateEventArgs
        {
            public EnginePointerMovedEventArgs PointerArgs { get; set; }
            public EngineUpdateEventArgs UpdateArgs { get; set; }
        }
    }

    public sealed class MouseEnterEngineEventArgs : EngineEventArgs
    {
        public MouseEnterEngineEventArgs(Engine engine) : base(engine)
        {

        }
    }

    public sealed class MouseLeaveEngineEventArgs : EngineEventArgs
    {
        public MouseLeaveEngineEventArgs(Engine engine) : base(engine)
        {

        }
    }
}
