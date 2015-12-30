using System.Numerics;

namespace SkiManager.Engine.Interfaces
{
    public interface ITransform
    {
        /// <summary>
        /// The absolute position of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The absolute scale of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        Vector2 Scale { get; set; }

        /// <summary>
        /// The absolute rotation of the <see cref="Entity"/>
        /// in degrees.
        /// </summary>
        float Rotation { get; set; }
    }
}
