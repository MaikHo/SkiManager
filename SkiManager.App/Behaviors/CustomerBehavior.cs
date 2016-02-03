using System;
using System.Linq;
using Newtonsoft.Json;
using SkiManager.Engine;
using SkiManager.App.Interfaces;

namespace SkiManager.App.Behaviors
{
    public sealed class CustomerBehavior : ReactiveBehavior, IItemBuyer
    {
        [JsonProperty]
        public Inventory Inventory { get; } = new Inventory();

        [JsonProperty]
        public ItemRequest CurrentItemRequest { get; private set; }

        [JsonProperty]
        private IGraphNode _myParkingLot;

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ParentChanged.Subscribe(OnParentChanged));
        }

        private void OnParentChanged(ParentChangedEngineEventArgs args)
        {
            if (args.Reason == Reasons.Unloaded.FromCar)
            {
                // unloaded on parking lot
                _myParkingLot = args.NewParent.GetImplementation<IGraphNode>();
                CurrentItemRequest = new ItemRequest(Items.SkiTicket, 1.0f);
                SetTargetToNextPointTowardsCashierOrCashierBooth();
            }
        }

        private void SetTargetToNextPointTowardsCashierOrCashierBooth()
        {
            var dijkstraValues = Entity.Parent.GetImplementation<IGraphNode>().GetDijkstraValues();
            var targetedCashier = Entity.Level.Entities
                .Where(_ => _.Implements<ICashier>() || _.ImplementsInChildren<ICashier>())
                .OrderBy(_ => dijkstraValues.Distances[_.GetImplementation<IGraphNode>()])
                .FirstOrDefault();

            if (targetedCashier == null)
            {
                // no cashier can be found -> return to car
                var pathToParkingLot = dijkstraValues.GetPathTowardsTarget(_myParkingLot);
                Entity.GetBehavior<MovableBehavior>().SetTarget(pathToParkingLot.Count > 1 ? pathToParkingLot[1].Entity : _myParkingLot.Entity);
            }

            var path = dijkstraValues.GetPathTowardsTarget(targetedCashier.GetImplementation<IGraphNode>());
            var movable = Entity.GetBehavior<MovableBehavior>();
            movable.SetTarget(path.Count > 1 ? path[1].Entity : targetedCashier);
        }
    }
}
