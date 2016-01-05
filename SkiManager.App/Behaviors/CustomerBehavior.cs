using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class CustomerBehavior : ReactiveBehavior
    {
        public Inventory Inventory { get; set; } = new Inventory();
    }
}
