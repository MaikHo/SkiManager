using System;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using SkiManager.App.Behaviors;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Features;

namespace SkiManager.App
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Engine.Engine.Current.Attach(Canvas);

            Engine.Engine.Current.AddFeature(new MouseInteractionFeature());

            var level = new Level();

            var coordinateSystem = new BasicCoordinateSystem();
            level.RootEntity.AddBehavior(coordinateSystem);

            var mapio = level.Instantiate(EntityTemplates.MapIO);
            mapio.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Circle, Color = Colors.Blue, Size = new Windows.Foundation.Size(15, 15) });
            mapio.GetBehavior<TransformBehavior>().Position = new Vector3(100, 0, 100);
            mapio.IsEnabled = true;

            var parkingLot = level.Instantiate(EntityTemplates.ParkingLot);
            parkingLot.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Square, Color = Colors.Gray, Size = new Windows.Foundation.Size(35, 25), FillGeometry = true });
            parkingLot.GetBehavior<TransformBehavior>().Position = new Vector3(250, 0, 250);
            parkingLot.GetBehavior<ParkingLotBehavior>().Slots = 100;
            parkingLot.IsEnabled = true;

            var road = level.Instantiate(EntityTemplates.Road);
            road.AddBehavior(
                new LineRendererBehavior
                {
                    Color = Colors.DarkGray,
                    RenderLayer = RenderLayers.Terrain
                });
            var roadB = road.GetBehavior<RoadBehavior>();
            roadB.Start = mapio.GetImplementation<IGraphNode>();
            roadB.End = parkingLot.GetImplementation<IGraphNode>();
            road.IsEnabled = true;

            var roadLotToCashier = level.Instantiate(EntityTemplates.Road);
            roadLotToCashier.Name = "Road between parking lot and cashier booth";
            roadLotToCashier.AddBehavior(
                new LineRendererBehavior
                {
                    Color = Colors.DarkGray,
                    RenderLayer = RenderLayers.Terrain
                });

            var cashierBooth = level.Instantiate(EntityTemplates.CashierBooth);
            cashierBooth.GetBehavior<TransformBehavior>().Position = new Vector3(250, 0, 400);
            cashierBooth.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Square, Color = Colors.Brown, Size = new Windows.Foundation.Size(15, 6), FillGeometry = true });
            cashierBooth.GetBehavior<SubgraphEntranceBehavior>().SubgraphNode = cashierBooth.GetImplementationInChildren<IWaitingQueue>().Entity.GetImplementation<IGraphNode>();
            roadB = roadLotToCashier.GetBehavior<RoadBehavior>();
            roadB.Start = parkingLot.GetImplementation<IGraphNode>();
            roadB.End = cashierBooth.GetImplementation<IGraphNode>();
            roadB.IsEnabled = true;
            cashierBooth.IsEnabled = true;

            foreach (var entity in level.Entities.Where(_ => _.Parent == null && !_.HasBehavior<DebugBehavior>()))
            {
                entity.AddBehavior(new DebugBehavior());
            }

            Engine.Engine.Current.LoadLevel(level);

            Engine.Engine.Current.Events.Update.Subscribe(async args =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => TimingBlock.Text = $"GameTime: {args.GameTime}{Environment.NewLine}DeltaTime: {args.DeltaTime}");
            });

            //Engine.Engine.Current.StartOrResume();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TerrainPage));
        }

        private void ToggleButton_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Engine.Engine.Current.StartOrResume();
        }

        private void ToggleButton_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Engine.Engine.Current.Pause();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Engine.Engine.Current?.CurrentLevel?.TimeScale != null)
            {
                Engine.Engine.Current.CurrentLevel.TimeScale = (float)e.NewValue;
            }
        }
    }
}
