using SkiManager.App.Behaviors;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App
{
    public static class EntityTemplates
    {
        public static Entity BasicRoad { get; }

        public static Entity Car { get; }

        public static Entity Customer { get; }

        public static Entity Road { get; }

        public static Entity MapIO { get; }

        public static Entity ParkingLot { get; }

        static EntityTemplates()
        {
            BasicRoad = new Entity();
            BasicRoad.AddBehavior(new TransformBehavior());
            BasicRoad.AddBehavior(new RoadBehavior());

            Car = new Entity();
            Car.AddBehavior(new TransformBehavior());
            Car.AddBehavior(new MovableBehavior());
            Car.AddBehavior(new CarBehavior());

            Customer = new Entity();
            Customer.AddBehavior(new TransformBehavior());
            Customer.AddBehavior(new MovableBehavior());
            Customer.AddBehavior(new CustomerBehavior());

            Road = new Entity();
            Road.AddBehavior(new TransformBehavior());
            Road.AddBehavior(new RoadBehavior());

            MapIO = new Entity();
            MapIO.AddBehavior(new TransformBehavior());
            MapIO.AddBehavior(new GraphConnectorNodeBehavior());
            MapIO.AddBehavior(new SpawnerBehavior());

            ParkingLot = new Entity();
            ParkingLot.AddBehavior(new TransformBehavior());
            ParkingLot.AddBehavior(new ParkingLotBehavior());
        }
    }
}
