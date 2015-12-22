using System;
using System.Numerics;
using System.Reactive.Linq;

using SkiManager.Engine;

namespace SkiManager.App
{
    public class TestBehavior : ReactiveBehavior
    {
        private static readonly Random _random = new Random();

        public TestBehavior()
        {
            var i = 0;
            //Engine.Engine.Current.Events.Update.Subscribe(OnUpdate);
            Update.Where(
                _ =>
                    {
                        i = (i + 1) % 100;
                        return i == 0;
                    }).Subscribe(OnUpdate);
        }

        private void OnUpdate(EngineUpdateEventArgs args)
        {
            Entity.Location = new GlobalLocation(new Vector2((float)(_random.NextDouble() * args.Sender.Size.Width), (float)(_random.NextDouble() * args.Sender.Size.Height)));
        }
    }
}
