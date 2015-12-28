using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using SkiManager.App.Behaviors;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkiManager.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Engine.Engine.Current.Attach(Canvas);

            var level = new Level();

            var mapio = level.Instantiate(EntityTemplates.MapIO);
            mapio.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Circle, Color = Colors.Blue, Size = new Windows.Foundation.Size(15, 15) });
            mapio.GetBehavior<TransformBehavior>().Position = new Vector2(100, 100);

            var parkingLot = level.Instantiate(EntityTemplates.ParkingLot);
            parkingLot.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Square, Color = Colors.Gray, Size = new Windows.Foundation.Size(35, 25), FillGeometry = true });
            parkingLot.GetBehavior<TransformBehavior>().Position = new Vector2(250, 250);
            parkingLot.GetBehavior<ParkingLotBehavior>().Slots = 100;

            var road = level.Instantiate(EntityTemplates.Road);
            road.AddBehavior(
                new LineRendererBehavior(
                    _ => _.GetBehavior<RoadBehavior>().Start.GetBehavior<TransformBehavior>().Position,
                    _ => _.GetBehavior<RoadBehavior>().End.GetBehavior<TransformBehavior>().Position)
                {
                    Color = Colors.DarkGray
                });
            var roadB = road.GetBehavior<RoadBehavior>();
            roadB.Start = mapio;
            roadB.End = parkingLot;

            Engine.Engine.Current.LoadLevel(level);

            Engine.Engine.Current.StartOrResume();
        }

        private void SelectNewTarget(TargetReachedEngineEventArgs args)
        {
            Engine.Engine.Current.CurrentLevel.Entities.First(_ => _.Name == "Movable")
                .GetBehavior<MovableBehavior>()
                .SetTarget(args.ReachedTarget.Name == "Node1"
                    ? Engine.Engine.Current.CurrentLevel.Entities.First(_ => _.Name == "Node2")
                    : Engine.Engine.Current.CurrentLevel.Entities.First(_ => _.Name == "Node1"));
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TerrainPage));
        }
    }
}
