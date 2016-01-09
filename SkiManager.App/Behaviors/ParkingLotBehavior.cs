using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class ParkingLotBehavior : GraphNodeBehavior
    {
        private readonly Dictionary<Entity, List<Entity>> _carToPassengerMappings = new Dictionary<Entity, List<Entity>>();

        public int Slots { get; set; }

        public int UsedSlots { get; private set; }

        public int FreeSlots => Slots - UsedSlots;

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ChildEnter.Where(a => a.Reason != Reasons.Unloaded.FromCar).Subscribe(OnChildEnter));
        }

        private void OnChildEnter(ChildEnterEngineEventArgs args)
        {
            if (args.EnteringChild.HasBehavior<CustomerBehavior>())
            {
                LoadCustomerIntoHisCarIfPossible(args);
            }
            else if (args.EnteringChild.HasBehavior<CarBehavior>())
            {
                ParkAndUnloadCarIfSlotsAreAvailable(args);
            }
            else
            {
                // unknown entity type -> disallow entering
                args.EnteringChild.SetParent(args.OldParent);
            }
        }

        private void ParkAndUnloadCarIfSlotsAreAvailable(ChildEnterEngineEventArgs args)
        {
            var car = args.EnteringChild.GetBehavior<CarBehavior>();
            // car enters
            if (FreeSlots == 0)
            {
                // disallow entering of car since there is no space, mark that it already tried this parkinglot
                car.TriedParkingLots.Add(Entity);
                args.EnteringChild.SetParent(args.OldParent, Reasons.NoSpace.InCarParkingLot);
                car.SetTargetToNextPointTowardsParkingLot(args.OldParent.GetImplementation<IGraphNode>());
                return;
            }

            // there is space -> unload passengers to IO node and increase used slots
            UsedSlots++;
            var passengers = new List<Entity>(car.Passengers);
            _carToPassengerMappings.Add(car.Entity, passengers);
            car.UnloadAllTo(Entity);
            car.Entity.IsEnabled = false;
        }

        private void LoadCustomerIntoHisCarIfPossible(ChildEnterEngineEventArgs args)
        {
            var correspondingEntry = _carToPassengerMappings.FirstOrDefault(_ => _.Value.Contains(args.EnteringChild));
            if (correspondingEntry.Key == null)
            {
                // entering customer does not have his car parked here -> disallow entering
                args.EnteringChild.SetParent(args.OldParent, Reasons.NotAllowed);
            }
            else
            {
                // car found -> load customer into it.
                var car = correspondingEntry.Key.GetBehavior<CarBehavior>();
                var success = car.TryLoad(args.EnteringChild);
                if (!success)
                {
                    // could not load into car -> set entity back
                    args.EnteringChild.SetParent(args.OldParent, Reasons.ProcessingUnsuccessful);
                    return;
                }

                // if all passengers for the car are loaded, enable the car
                if (correspondingEntry.Value.All(p => car.Passengers.Contains(p)))
                {
                    // car is fully loaded -> enable it
                    car.Entity.IsEnabled = true;
                }
            }
        }
    }
}
