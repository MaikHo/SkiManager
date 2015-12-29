using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Sprites;
using System;
using System.Numerics;
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
            spriteManager.Sprites.Add("Terrain.Rock2", new Uri("ms-appx:///Assets/Sprites/terrain-cliffs-ground.png"), new Vector2(4000, 4000));
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

            Engine.Engine.Current.LoadLevel(level);
            Engine.Engine.Current.StartOrResume();
        }
        
        private void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scaleChangeRatio = canvas.DpiScale / scrollViewer.ZoomFactor;

            if (e == null || !e.IsIntermediate || scaleChangeRatio <= 0.8 || scaleChangeRatio >= 1.25)
            {
                canvas.DpiScale = scrollViewer.ZoomFactor;
            }
        }
    }
}
