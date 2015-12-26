using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Features;
using System;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SkiManager.App
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class TerrainTestPage : Page
    {
        public TerrainTestPage()
        {
            InitializeComponent();

            Engine.Engine.Current.Attach(canvas);

            var level = new Level();

            Engine.Engine.Current.AddFeature(new TrackMousePositionEngineFeature(true));

            var terrain = new TerrainBehavior();

            Engine.Engine.Current.LoadLevel(level);

            Engine.Engine.Current.StartOrResume();
        }
    }
}
