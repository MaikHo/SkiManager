using SkiManager.App.Behaviors;
using SkiManager.App.Features;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Interfaces;
using SkiManager.Engine.Sprites;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SkiManager.App
{
    public sealed partial class TerrainPage : Page
    {
        public TerrainPage()
        {
            /*
            HEIGHT MAP INFO:
            - Kitzbüheler Alpen: 20x20 km, BaseHeight: 719 m, MaxHeight: 2328 m
            - Wildkogel        :  8x8  km, BaseHeight: 779 m, MaxHeight: 2264 m
            - Zillertal Arena  : 20x20 km, BaseHeight: 524 m, MaxHeight: 3049 m
            */

            InitializeComponent();

            Engine.Engine.Current.Attach(canvas);

            var level = new Level();

            var spriteManager = new SpriteManagerBehavior();
            spriteManager.Sprites.Add("Terrain.Grass", new Uri("ms-appx:///Assets/Sprites/Grass0202_1_S.jpg"), new Vector2(20, 20));
            spriteManager.Sprites.Add("Terrain.Snow", new Uri("ms-appx:///Assets/Sprites/Snow0080_1_S.jpg"), new Vector2(20, 20));
            spriteManager.Sprites.Add("Terrain.Rock", new Uri("ms-appx:///Assets/Sprites/terrain-cliffs-ground.png"), new Vector2(20, 20));
            spriteManager.Sprites.Add("Terrain.HeightMap", new Uri("ms-appx:///Assets/Sprites/HeightMaps/Wildkogel8x8 Height Map (Merged).png"), new Vector2(8000, 8000));

            var terrain = new TerrainBehavior
            {
                Height = 2264,
                BaseHeight = 779,
                HeightMap = "Terrain.HeightMap"
            };

            var terrainRenderer = new TerrainRendererBehavior
            {
                GrassSprite = "Terrain.Grass",
                SnowSprite = "Terrain.Snow",
                RockSprite = "Terrain.Rock"
            };
            
            level.RootEntity.AddBehavior(spriteManager);
            level.RootEntity.AddBehavior(terrain);
            level.RootEntity.AddBehavior(terrainRenderer);

            Engine.Engine.Current.AddFeature(new TrackTerrainMousePositionEngineFeature(terrain, true));

            Engine.Engine.Current.LoadLevel(level);
            Engine.Engine.Current.StartOrResume();

            canvas.CreateResources += Canvas_CreateResources;
        }

        private async void Canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasVirtualControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            await Task.Delay(1000);

            AddRoads(
                Engine.Engine.Current.CurrentLevel.RootEntity,
                Engine.Engine.Current.CurrentLevel.RootEntity.GetImplementation<ICoordinateSystem>());
        }

        private void AddRoads(Entity container, ICoordinateSystem coords)
        {
            var points = new[]
            {
                coords.Transform2DTo3D(new Vector2(0, 7095)),
                coords.Transform2DTo3D(new Vector2(1977, 6464)),
                coords.Transform2DTo3D(new Vector2(4701, 5895)),
                coords.Transform2DTo3D(new Vector2(6330, 4602)),
                coords.Transform2DTo3D(new Vector2(8000, 3654))
            };

            var connectors = points.Select(p =>
            {
                var conn = container.Level.InstantiateAndLoad(EntityTemplates.GraphConnector, container);
                conn.GetBehavior<TransformBehavior>().Position = p;
                return conn;
            }).ToArray();

            var roads = Enumerable.Range(0, connectors.Length - 1).Select(i =>
            {
                var road = container.Level.InstantiateAndLoad(EntityTemplates.Road, container);
                road.AddBehavior(new LineRendererBehavior(
                    e => e.GetBehavior<GraphEdgeBehavior>().Start.GetBehavior<TransformBehavior>().Position,
                    e => e.GetBehavior<GraphEdgeBehavior>().End.GetBehavior<TransformBehavior>().Position));
                var edge = road.GetBehavior<GraphEdgeBehavior>();
                edge.Start = connectors[i];
                edge.End = connectors[i + 1];
                return road;
            }).ToArray();

            Engine.Engine.Current.LoadLevel(Engine.Engine.Current.CurrentLevel);
        }
        
        private void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scaleChangeRatio = canvas.DpiScale / scrollViewer.ZoomFactor;

            if (e == null || !e.IsIntermediate || scaleChangeRatio <= 0.8 || scaleChangeRatio >= 1.25)
            {
                canvas.DpiScale = scrollViewer.ZoomFactor;
            }
        }

        private void canvas_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            canvas.Invalidate();
        }
    }
}
