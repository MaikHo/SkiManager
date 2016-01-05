using System.Collections.Generic;
using System.Linq;

namespace SkiManager.Engine
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Gets the full path of an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The full path of an entity.</returns>
        public static string GetPath(this Entity entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            return (entity.Parent?.GetPath() ?? string.Empty + "/" + entity.Name).TrimStart('/');
        }

        /// <summary>
        /// Determines whether the specified entity has an instance of the given behavior type.
        /// </summary>
        /// <typeparam name="TBehavior">The type of the behavior.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>True, if the entity contains a behavior of the given type. False, otherwise.</returns>
        public static bool HasBehavior<TBehavior>(this Entity entity) where TBehavior : ReactiveBehavior => entity.Implements<TBehavior>();

        /// <summary>
        /// Gets the requested behavior, or null, if this entity does not contain the behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The type of the behavior.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The behavior or null, if it is not attached.</returns>
        public static TBehavior GetBehavior<TBehavior>(this Entity entity) where TBehavior : ReactiveBehavior => entity.Behaviors.OfType<TBehavior>().FirstOrDefault();

        /// <summary>
        /// Determines whether the specified entity has any behavior that implements the given interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>True, if the specified entity has any behavior that implements the given interface. False otherwise.</returns>
        public static bool Implements<T>(this Entity entity) => entity.Behaviors.OfType<T>().Any();

        /// <summary>
        /// Gets the behavior that implements the given interface, or null, if no behavior implements it.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The behavior that implements the given interface, or null, if no behavior implements it.</returns>
        public static T GetImplementation<T>(this Entity entity) => entity.Behaviors.OfType<T>().FirstOrDefault();

        /// <summary>
        /// Gets an enumeration of all behaviors that implement the given interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>An enumeration of all behaviors that implement the given interface.</returns>
        public static IEnumerable<T> GetImplementations<T>(this Entity entity) => entity.Behaviors.OfType<T>().ToList();
    }
}
