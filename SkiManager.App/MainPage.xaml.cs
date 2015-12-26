using System;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using SkiManager.App.Behaviors;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Features;

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

            Engine.Engine.Current.AddFeature(new TrackMousePositionEngineFeature(true));
            var entity = new Entity { Name = "Something" };
            entity.AddBehavior(new TransformBehavior { Position = new Vector2(100) });
            entity.AddBehavior(new GraphNodeBehavior());
            entity.AddBehavior(new ShapeColliderBehavior { ColliderTypes = ColliderType.Pointer, Size = new Windows.Foundation.Size(25, 25) });
            entity.AddBehavior(new TestBehavior());
            entity.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Square, Size = new Windows.Foundation.Size(25, 25) });

            level.AddEntity(entity);

            var node1 = new Entity { Name = "Node1" };
            node1.AddBehavior(new TransformBehavior { Position = new Vector2(200) });
            node1.AddBehavior(new GraphNodeBehavior());
            node1.AddBehavior(new ShapeColliderBehavior { ColliderTypes = ColliderType.Pointer, Size = new Windows.Foundation.Size(15, 15) });
            node1.AddBehavior(new TestBehavior());
            node1.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Circle, Size = new Windows.Foundation.Size(15, 15), Color = Colors.Blue });
            level.AddEntity(node1);

            var node2 = new Entity { Name = "Node2" };
            node2.AddBehavior(new TransformBehavior { Position = new Vector2(200, 500) });
            node2.AddBehavior(new ShapeColliderBehavior { ColliderTypes = ColliderType.Pointer, Size = new Windows.Foundation.Size(15, 15) });
            node2.AddBehavior(new TestBehavior());
            node2.AddBehavior(new GraphNodeBehavior());
            node2.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Circle, Size = new Windows.Foundation.Size(15, 15), Color = Colors.Blue });
            level.AddEntity(node2);

            var edge = new Entity { Name = "Edge" };
            edge.AddBehavior(new GraphEdgeBehavior { Start = node1, End = node2 });
            edge.AddBehavior(new LineRendererBehavior(_ => _.GetBehavior<GraphEdgeBehavior>().Start.GetBehavior<TransformBehavior>().Position,
                _ => _.GetBehavior<GraphEdgeBehavior>().End.GetBehavior<TransformBehavior>().Position)
            { Color = Colors.Blue });
            level.AddEntity(edge);

            edge.GetBehavior<GraphEdgeBehavior>().Start = node1;
            edge.GetBehavior<GraphEdgeBehavior>().End = node2;
            node1.GetBehavior<GraphNodeBehavior>().AdjacentEdges.Add(edge);
            node2.GetBehavior<GraphNodeBehavior>().AdjacentEdges.Add(edge);

            var movable = new Entity { Name = "Movable" };
            movable.AddBehavior(new TransformBehavior());
            movable.AddBehavior(new SimpleGeometryRendererBehavior { Geometry = SimpleGeometry.Circle, Color = Colors.Lime, Size = new Windows.Foundation.Size(5, 5) });
            movable.AddBehavior(new MovableBehavior());
            level.AddEntity(movable);
            movable.GetBehavior<MovableBehavior>().SetTarget(node1);
            movable.GetBehavior<MovableBehavior>().TargetReached.Subscribe(SelectNewTarget);

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
    }
}
