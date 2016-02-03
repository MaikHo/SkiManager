using System.Collections.Generic;

namespace SkiManager.App.Interfaces
{
    public interface IItemSeller
    {
        IReadOnlyList<Item> SoldItems { get; }

        Cost GetCostForItem(Item item);
    }

    public struct Cost
    {
        public static Cost None { get; } = new Cost();

        public static Cost Infinite { get; } = new Cost();

        public Item Item { get; }

        public float Amount { get; }

        public Cost(Item item, float amount)
        {
            Item = item;
            Amount = amount;
        }
    }
}
