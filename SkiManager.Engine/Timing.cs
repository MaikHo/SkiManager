using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SkiManager.Engine
{
    public static class Timing
    {
        public static async Task Delay(TimeSpan delayGameTime)
        {
            var start = Engine.Current.CurrentLevel.GameTime;
            await Engine.Current.Events.Update.Where(args => args.GameTime - start > delayGameTime).FirstAsync();
        }

        public static async Task DelayUntil(Func<bool> condition)
        {
            // TODO prevent this from hanging when engine is shutting down
            await Engine.Current.Events.Update.Where(_ => condition() && !Engine.Current.Status.IsPaused).FirstAsync();
        }

        public static async Task DelayUntilWithTimeout(Func<bool> condition, TimeSpan gameTimeTimeout)
        {
            var start = Engine.Current.CurrentLevel.GameTime;
            // TODO prevent this from hanging when engine is shutting down
            await Engine.Current.Events.Update.Where(args => (condition() || (args.GameTime - start > gameTimeTimeout)) && !Engine.Current.Status.IsPaused).FirstAsync();
        }
    }
}
