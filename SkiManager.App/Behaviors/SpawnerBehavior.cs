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
                 i = i + 1 % 100;
                 return i == 0;
             }).Subscribe(OnUpdate);
        }

        protected override void Unloading()
        {
            _subscription.Dispose();
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            var carEntity = Entity.Level.Instantiate(EntityTemplates.Car);
            var car = carEntity.GetBehavior<CarBehavior>();
            var passengerCount = new Random().Next(5) + 1;
            car.Slots = passengerCount;
            for (int i = 0; i < passengerCount; i++)
            {
                car.TryLoad(Entity.Level.Instantiate(EntityTemplates.Customer));
            }
            carEntity.SetParent(Entity); // this effectively requires GraphNodeBehavior
            car.SetTargetToNextPointTowardsParkingLot(Entity.GetImplementation<IGraphNode>());
        }
    }
}
