using SkiManager.Engine.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SkiManager.Engine.Sprites
{
    public class SpriteManagerBehavior : ReactiveBehavior
    {
        public SpriteCollection Sprites { get; } = new SpriteCollection();

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(EarlyCreateResources.Subscribe(e => e.Tasks.Add(OnCreateResourcesAsync(e))));
        }

        private async Task OnCreateResourcesAsync(EngineCreateResourcesEventArgs e)
        {
            // Load all registered sprites

            var coords = Entity.GetImplementation<ICoordinateSystem>();
            await Task.WhenAll(Sprites.Select(sprite => sprite.LoadAsync(e.Sender, coords))).ConfigureAwait(false);
        }
    }
}
