using System.Collections.Generic;

namespace SkiManager.Engine
{
    public class Entity
    {
        private readonly List<ReactiveBehavior> _behaviors = new List<ReactiveBehavior>();
        public IReadOnlyList<ReactiveBehavior> Behaviors => _behaviors;

        public bool IsEnabled { get; set; } = true;

        public ILocation Location { get; set; }

        public Entity()
        {

        }

        public void AddBehavior(ReactiveBehavior behavior)
        {
            behavior.Attach(this);
            _behaviors.Add(behavior);
        }

        public void RemoveBehavior(ReactiveBehavior behavior)
        {
            _behaviors.Remove(behavior);
            behavior.Detach(this);
        }
    }
}
