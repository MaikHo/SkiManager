using System;
using System.Collections.Generic;
using System.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresBehavior(typeof(MovableBehavior))]
    public sealed class CarBehavior : TransporterBaseBehavior
    {
        private IDisposable _updateSubscription;
        private IDisposable _targetReachedSubscription;
        public bool HasBeenParkedAtSomeTime { get; private set; }

        public List<Entity> TriedParkingLots { get; } = new List<Entity>();

        protected override void Loaded()
        {
            _updateSubscription = Update.Subscribe(OnUpdate);
            _targetReachedSubscription = Entity.GetBehavior<MovableBehavior>().TargetReached.Subscribe(OnTargetReached);
            SetTargetToNextPointTowardsParkingLot(Entity.Parent.GetImplementation<IGraphNode>());
        }

        protected override void Unloading()
        {
            _updateSubscription.Dispose();
            _targetReachedSubscription.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            HasBeenParkedAtSomeTime |= IsParked;
        }

        private void OnTargetReached(TargetReachedEngineEventArgs args)
        {
            if (!IsParked && !HasBeenParkedAtSomeTime)
            {
                // this car only spawned, it has never been parked -> target towards parkinglot
                SetTargetToNextPointTowardsParkingLot(args.ReachedTarget.GetImplementation<IGraphNode>());
            }
            else if (!IsParked && HasBeenParkedAtSomeTime)
            {
                // car was parked before -> target map exit
                SetTargetToNextPointTowardsRandomOfClosestThreeMapExits(args.ReachedTarget.GetImplementation<IGraphNode>());
            }
        }

        internal void SetTargetToNextPointTowardsParkingLot(IGraphNode currentNode)
        {
            var dijkstraValues = currentNode.GetDijkstraValues();
            var targetedParkingLot = Entity.Level.Entities
                .Where(_ => _.HasBehavior<ParkingLotBehavior>())
                .Where(_ => !TriedParkingLots.Contains(_))
                .OrderBy(_ => dijkstraValues.Distances[_.GetImplementation<IGraphNode>()])
                .FirstOrDefault();

            if (targetedParkingLot == null)
            {
                // no more parkinglots to try -> leave frustrated ;D
                SetTargetToNextPointTowardsRandomOfClosestThreeMapExits(currentNode);
            }

            var current = targetedParkingLot.GetImplementation<IGraphNode>();
            var path = new List<Entity> { targetedParkingLot };
            while (dijkstraValues.Predecessors[current] != null)
            {
                current = dijkstraValues.Predecessors[current];
                path.Insert(0, current.Entity);
            }
            var movable = Entity.GetBehavior<MovableBehavior>();
            movable.SetTarget(path.Count > 1 ? path[1] : targetedParkingLot);
        }

        internal void SetTargetToNextPointTowardsRandomOfClosestThreeMapExits(IGraphNode currentNode)
        {
            var dijkstraValues = currentNode.GetDijkstraValues();
            var possibleExits = Entity.Level.Entities
                .Where(_ => _.Implements<IMapExit>())
                .OrderBy(_ => dijkstraValues.Distances[_.GetImplementation<IGraphNode>()])
                .Take(3)
                .ToList();

            if (possibleExits.Count == 0)
            {
                throw new InvalidLevelConfigurationException("Map does not contain any exits.");
            }

            var targetedExit = possibleExits[new Random().Next(possibleExits.Count)];
            var current = targetedExit.GetImplementation<IGraphNode>();
            var path = new List<Entity> { targetedExit };
            while (dijkstraValues.Predecessors[current] != null)
            {
                current = dijkstraValues.Predecessors[current];
                path.Insert(0, current.Entity);
            }
            var movable = Entity.GetBehavior<MovableBehavior>();
            movable.SetTarget(path[1]);
        }
    }
}
