using System.Numerics;
using SkiManager.Engine.Interfaces;

namespace SkiManager.Engine.Behaviors
{
    public sealed class LineTransformBehavior : ReactiveBehavior, ILineTransform, IReadOnlyTransform
    {
        /// <summary>
        /// Gets or sets the first point.
        /// </summary>
        public Vector3 Point1 { get; set; }

        /// <summary>
        /// Gets or sets the second point.
        /// </summary>
        public Vector3 Point2 { get; set; }

        /// <summary>
        /// Gets the center position of this line transform.
        /// </summary>
        public Vector3 Position => Point1 + 0.5f * Vector3.Subtract(Point2, Point1);
    }
}
