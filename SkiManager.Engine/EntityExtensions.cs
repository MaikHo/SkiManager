using System.Collections.Generic;
using System.Linq;

namespace SkiManager.Engine
{
    public static class EntityExtensions
    {
        public static string GetPath(this Entity entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            return (entity.Parent?.GetPath() ?? string.Empty + "/" + entity.Name).TrimStart('/');
        }
        public static bool HasBehavior<TBehavior>(this Entity entity) where TBehavior : ReactiveBehavior => entity.Implements<TBehavior>();

        public static TBehavior GetBehavior<TBehavior>(this Entity entity) where TBehavior : ReactiveBehavior => entity.Behaviors.OfType<TBehavior>().FirstOrDefault();

        public static bool Implements<T>(this Entity entity) => entity.Behaviors.OfType<T>().Any();

        public static T GetImplementation<T>(this Entity entity) => entity.Behaviors.OfType<T>().FirstOrDefault();

        public static IEnumerable<T> GetImplementations<T>(this Entity entity) => entity.Behaviors.OfType<T>().ToList();
    }
}
