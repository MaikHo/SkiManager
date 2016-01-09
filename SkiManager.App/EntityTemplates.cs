using Windows.UI;
using SkiManager.App.Behaviors;
using SkiManager.App.Interfaces;
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

        public static Entity SingleCashier { get; }

        public static Entity WaitingQueue { get; }

        public static Entity CashierBooth { get; }

        static EntityTemplates()
        {
            Car = new Entity { Name = nameof(Car) };
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

            Customer = new Entity { Name = nameof(Customer) };
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
            Customer.GetBehavior<CustomerBehavior>().Inventory.AddItem(Items.Money, 100);

            Road = new Entity { Name = nameof(Road) };
            Road.AddBehavior(new TransformBehavior());
            Road.AddBehavior(new RoadBehavior());

            MapIO = new Entity { Name = nameof(MapIO) };
            MapIO.AddBehavior(new TransformBehavior());
            MapIO.AddBehavior(new GraphConnectorNodeBehavior());
            MapIO.AddBehavior(new SpawnerBehavior());

            ParkingLot = new Entity { Name = nameof(ParkingLot) };
            ParkingLot.AddBehavior(new TransformBehavior());
            ParkingLot.AddBehavior(new ParkingLotBehavior());

            GraphConnector = new Entity { Name = nameof(GraphConnector) };
            GraphConnector.AddBehavior(new TransformBehavior());
            GraphConnector.AddBehavior(new GraphConnectorNodeBehavior());

            SingleCashier = new Entity { Name = nameof(SingleCashier) };
            SingleCashier.AddBehavior(new TransformBehavior());
            SingleCashier.AddBehavior(new GraphConnectorNodeBehavior());
            SingleCashier.AddBehavior(new CashierBehavior());

            WaitingQueue = new Entity { Name = nameof(WaitingQueue) };
            WaitingQueue.AddBehavior(new TransformBehavior());
            WaitingQueue.AddBehavior(new GraphConnectorNodeBehavior());
            WaitingQueue.AddBehavior(new WaitingQueueBehavior());

            CashierBooth = new Entity { Name = nameof(CashierBooth) };
            CashierBooth.AddBehavior(new TransformBehavior());
            CashierBooth.AddBehavior(new GraphConnectorNodeBehavior());
            CashierBooth.AddBehavior(new SubgraphEntranceBehavior());
            var cbQueue = WaitingQueue.Clone();
            cbQueue.Name = "CashierBooth.WaitingQueue";
            var cbqQueueBehavior = cbQueue.GetBehavior<WaitingQueueBehavior>();
            cbqQueueBehavior.MaxQueueSize = 10;
            cbQueue.SetParent(CashierBooth, Reasons.TemplateCreation);
            var cbCashier1 = SingleCashier.Clone();
            cbCashier1.Name = "CashierBooth.Cashier1";
            var cbc1CashierBehavior = cbCashier1.GetBehavior<CashierBehavior>();
            cbc1CashierBehavior.TicketPrice = 42;
            cbc1CashierBehavior.NextNode = CashierBooth.GetImplementation<IGraphNode>();
            cbc1CashierBehavior.MinimumProcessingSeconds = 1;
            cbc1CashierBehavior.MaximumProcessingSeconds = 5;
            cbc1CashierBehavior.UseWaitingQueueOfParent = true;
            cbCashier1.SetParent(CashierBooth, Reasons.TemplateCreation);
            CashierBooth.GetBehavior<SubgraphEntranceBehavior>().SubgraphNode = cbQueue.GetImplementation<IGraphNode>();

            // TODO remove debug code
            Car.AddBehavior(new DebugBehavior());
            Customer.AddBehavior(new DebugBehavior());
            Road.AddBehavior(new DebugBehavior());
            MapIO.AddBehavior(new DebugBehavior());
            ParkingLot.AddBehavior(new DebugBehavior());
            GraphConnector.AddBehavior(new DebugBehavior());
            SingleCashier.AddBehavior(new DebugBehavior());
            WaitingQueue.AddBehavior(new DebugBehavior());
            CashierBooth.AddBehavior(new DebugBehavior());
        }
    }
}
