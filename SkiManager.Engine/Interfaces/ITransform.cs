using System.Numerics;

namespace SkiManager.Engine.Interfaces
{
    public interface IReadOnlyTransform
    {
        /// <summary>
        /// Gets the absolute position of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        Vector3 Position { get; }
    }

    public interface ITransform : IReadOnlyTransform
    {
        /// <summary>
        /// The absolute position of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        new Vector3 Position { get; set; }

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

    public interface ILineTransform
    {
        /// <summary>
        /// Gets or sets the first point.
        /// </summary>
        Vector3 Point1 { get; set; }

        /// <summary>
        /// Gets or sets the second point.
        /// </summary>
        Vector3 Point2 { get; set; }
    }
}
