namespace SkiManager.App.Interfaces
{
    public interface IItemBuyer : IInventoryCarrier
    {
        ItemRequest CurrentItemRequest { get; }
    }

    public struct ItemRequest
    {
        public static ItemRequest None { get; } = new ItemRequest();

        public Item Item { get; }

        public float Amount { get; }

        public ItemRequest(Item item, float amount)
        {
            Item = item;
            Amount = amount;
        }
    }
}
