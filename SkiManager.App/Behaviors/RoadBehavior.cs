namespace SkiManager.App.Behaviors
{
    public sealed class RoadBehavior : GraphEdgeBehavior
    {
        public RoadBehavior()
        {
            IsBidirectional = true;
            BaseSpeedModifier = 1.2f;
        }
    }
}
