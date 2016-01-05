namespace SkiManager.App
{
    public static class Items
    {
        public static Item Money { get; } = new Item("Money", ItemUnit.Continuous, 0);
        public static Item SkiTicket { get; } = new Item("SkiTicket", ItemUnit.Discrete, 0.01f);
    }
}
