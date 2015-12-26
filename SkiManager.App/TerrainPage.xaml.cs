using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            Engine.Engine.Current.Attach(Canvas);

            var level = new Level();

            var spriteManager = new SpriteManagerBehavior();
            spriteManager.Sprites.Add("Terrain.Grass", new Uri("ms-appx:///Assets/Sprites/grass.jpg"), new Vector2(2, 2));
            spriteManager.Sprites.Add("Terrain.Snow", new Uri("ms-appx:///Assets/Sprites/snow(deep).png"), new Vector2(2, 2));
            spriteManager.Sprites.Add("Terrain.Rock", new Uri("ms-appx:///Assets/Sprites/terrain-cliffs-ground.jpg"), new Vector2(2, 2));

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
    }
}
