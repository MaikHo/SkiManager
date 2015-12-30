using Windows.UI;
using SkiManager.App.Behaviors;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App
{
    public static class EntityTemplates
    {
        public static Entity Car { get; }

        public static Entity Customer { get; }

        public static Entity Road { get; }

        public static Entity MapIO { get; }

        public static Entity ParkingLot { get; }

        public static Entity GraphConnector { get; }

        static EntityTemplates()
        {
            Car = new Entity();
            Car.AddBehavior(new TransformBehavior());
            Car.AddBehavior(new MovableBehavior { Speed = 0.5f });
            Car.AddBehavior(new CarBehavior());
            Car.AddBehavior(new SimpleGeometryRendererBehavior
            {
                Geometry = SimpleGeometry.Square,
                DrawCenter = false,
                Color = Colors.DarkGreen,
                FillGeometry = true,
                Size = new Windows.Foundation.Size(6, 2)
            });

            Customer = new Entity();
            Customer.AddBehavior(new TransformBehavior());
            Customer.AddBehavior(new MovableBehavior());
            Customer.AddBehavior(new CustomerBehavior());
            Customer.AddBehavior(new SimpleGeometryRendererBehavior
            {
                Geometry = SimpleGeometry.Circle,
                DrawCenter = false,
                Color = Colors.Cyan,
                FillGeometry = true,
                Size = new Windows.Foundation.Size(1, 1)
            });

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

            GraphConnector = new Entity();
            GraphConnector.AddBehavior(new TransformBehavior());
            GraphConnector.AddBehavior(new GraphConnectorNodeBehavior());
        }
    }
}
