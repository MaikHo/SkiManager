using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class ParkingLotBehavior : ReactiveBehavior
    {
        private IDisposable _subscription;
        private Dictionary<Entity, List<Entity>> _carToPassengerMappings = new Dictionary<Entity, List<Entity>>();

        public int Slots { get; set; }

        public int UsedSlots { get; private set; }

        public int FreeSlots => Slots - UsedSlots;

        public Entity SkiAreaIONode { get; set; }

        public Entity RoadIONode { get; set; }

        protected override void Loaded()
        {
            _subscription = ChildEnter.Subscribe(OnChildEnter);
        }

        protected override void Unloading()
        {
            _subscription?.Dispose();
        }

        private void OnChildEnter(ChildEnterEngineEventArgs args)
        {
            if (args.OldParent == SkiAreaIONode)
            {
                // entity enters from skiarea -> if it has a car, load it, otherwise deny entering
                if (args.EnteringChild.HasBehavior<CustomerBehavior>())
                {
                    var correspondingEntry =
                        _carToPassengerMappings.FirstOrDefault(_ => _.Value.Contains(args.EnteringChild));
                    if (correspondingEntry.Key == null)
                    {
                        // entering customer does not have his car parked here -> disallow entering
                        args.EnteringChild.SetParent(args.OldParent);
                    }
                    else
                    {
                        // car found -> load customer into it.
                        var car = correspondingEntry.Key.GetBehavior<CarBehavior>();
                        var success = car.TryLoad(args.EnteringChild);
                        if (!success)
                        {
                            // could not load into car -> set entity back
                            args.EnteringChild.SetParent(args.OldParent);
                            return;
                        }

                        // if all passengers for the car are loaded, set the car to the road io node
                        if (correspondingEntry.Value.All(p => car.Passengers.Contains(p)))
                        {
                            // car is fully loaded
                            car.Entity.SetParent(RoadIONode);
                        }
                    }
                }
                else
                {
                    // unknown entity type -> disallow entering
                    args.EnteringChild.SetParent(args.OldParent);
                }
            }
            else
            {
                // entity enters from somewhere else
                if (args.EnteringChild.HasBehavior<CarBehavior>())
                {
                    // car enters
                    if (FreeSlots == 0)
                    {
                        // disallow entering of car since there is no space
                        args.EnteringChild.SetParent(args.OldParent);
                        return;
                    }

                    // there is space -> unload passengers to IO node and increase used slots
                    UsedSlots++;
                    var car = args.EnteringChild.GetBehavior<CarBehavior>();
                    var passengers = new List<Entity>(car.Passengers);
                    _carToPassengerMappings.Add(car.Entity, passengers);
                    car.UnloadAllTo(SkiAreaIONode);
                }
                else
                {
                    // unknown entity type -> disallow entering
                    args.EnteringChild.SetParent(args.OldParent);
                }
            }
        }
    }
}
