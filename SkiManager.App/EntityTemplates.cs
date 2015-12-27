using SkiManager.App.Behaviors;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App
{
    public static class EntityTemplates
    {
        public static Entity BasicRoad { get; }

        public static Entity Car { get; }

        public static Entity Customer { get; }

        static EntityTemplates()
        {
            BasicRoad = new Entity();
            BasicRoad.AddBehavior(new TransformBehavior());
            BasicRoad.AddBehavior(new RoadBehavior());
        }
    }
}
