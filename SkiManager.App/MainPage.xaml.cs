using System.Numerics;

using Windows.UI.Xaml.Controls;

using SkiManager.Engine;
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

            Engine.Engine.Current.AddFeature(new TrackMousePositionEngineFeature(true));
            var entity = new Entity();
            entity.Location = new GlobalLocation(Vector2.One);
            entity.AddBehavior(new TestBehavior());
            Engine.Engine.Current.AddFeature(new DebugRenderEntityEngineFeature(entity));

            Engine.Engine.Current.StartOrResume();
        }
    }
}
