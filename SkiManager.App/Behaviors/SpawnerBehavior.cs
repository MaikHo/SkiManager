using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SpawnerBehavior : ReactiveBehavior
    {
        private IDisposable _subscription;

        protected override void Loaded()
        {
            var i = 0;
            _subscription = Update.Where(_ =>
            {
                var result = i == 0;
                i = (i + 1) % 200;
                return result;
            }).Subscribe(OnUpdate);
        }

        protected override void Unloading()
        {
            _subscription.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            var carEntity = Entity.Level.InstantiateAndLoad(EntityTemplates.Car, Entity);
            carEntity.GetBehavior<MovableBehavior>().SetLastTarget(Entity);
            var car = carEntity.GetBehavior<CarBehavior>();
            var passengerCount = new Random().Next(5) + 1;
            car.Slots = passengerCount;
            for (var i = 0; i < passengerCount; i++)
            {
                car.TryLoad(Entity.Level.InstantiateAndLoad(EntityTemplates.Customer, carEntity));
            }
            carEntity.IsEnabled = true;
        }
    }
}
