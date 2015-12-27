using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using System;
using System.Numerics;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SkiManager.App
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class TerrainPage : Page
    {
        public TerrainPage()
        {
            InitializeComponent();

            Engine.Engine.Current.Attach(canvas);

            var level = new Level();

            var spriteManager = new SpriteManagerBehavior();
            spriteManager.Sprites.Add("Terrain.Grass", new Uri("ms-appx:///Assets/Sprites/grass.jpg"), new Vector2(10, 10));
            spriteManager.Sprites.Add("Terrain.Snow", new Uri("ms-appx:///Assets/Sprites/snow(deep).png"), new Vector2(10, 10));
            spriteManager.Sprites.Add("Terrain.Rock", new Uri("ms-appx:///Assets/Sprites/terrain-cliffs-ground.png"), new Vector2(2, 2));

            var terrain = new TerrainBehavior
            {
                BaseHeight = 719,
                Size = new Vector3(20000, 2328, 20000),
                HeightMapSource = new Uri("ms-appx:///Assets/Sprites/Kitzbüheler Alpen Height Map (Merged).png")
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
