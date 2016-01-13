namespace SkiManager.App.Behaviors
{
    public sealed class SlopeBehavior : GraphEdgeBehavior
    {
        public SlopeDifficulty Difficulty { get; set; }
    }

    public enum SlopeDifficulty
    {
        Easy,
        Medium,
        Hard
    }
}
