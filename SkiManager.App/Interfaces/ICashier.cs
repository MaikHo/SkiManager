namespace SkiManager.App.Interfaces
{
    public interface ICashier : IItemSeller
    {
        int MinimumProcessingSeconds { get; set; }

        int MaximumProcessingSeconds { get; set; }

        bool IsProcessing { get; }

        IGraphNode NextNode { get; set; }
    }
}
