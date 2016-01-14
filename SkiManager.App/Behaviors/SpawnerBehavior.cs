using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SpawnerBehavior : ReactiveBehavior
    {
        private TimeSpan _timeSinceLastSpawn = TimeSpan.FromSeconds(15);

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(Update.Where(a =>
            {
                _timeSinceLastSpawn += a.DeltaTime;
                var result = _timeSinceLastSpawn.TotalSeconds > 20;
                if (result)
                    _timeSinceLastSpawn = TimeSpan.Zero;
                return result;
            }).Subscribe(OnUpdate));
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            var carEntity = Entity.Level.Instantiate(EntityTemplates.Car, Entity);
            carEntity.GetBehavior<MovableBehavior>().SetLastTarget(Entity);
            var car = carEntity.GetBehavior<CarBehavior>();
            var passengerCount = new Random().Next(5) + 1;
            car.Slots = passengerCount;
            for (var i = 0; i < passengerCount; i++)
            {
                car.TryLoad(Entity.Level.Instantiate(EntityTemplates.Customer, carEntity));
            }
            carEntity.IsEnabled = true;
        }
    }
}
