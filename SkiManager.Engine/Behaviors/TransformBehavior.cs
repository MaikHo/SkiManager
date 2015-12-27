using System.Numerics;

using SkiManager.Engine.Interfaces;
using System;

namespace SkiManager.Engine.Behaviors
{
    /// <summary>
    /// Represents the position, rotation and scale in world space.
    /// </summary>
    public sealed class TransformBehavior : ReactiveBehavior, ITransform
    {
        /// <summary>
        /// The absolute position of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The absolute scale of the <see cref="Entity"/>
        /// in world space.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        /// The absolute rotation of the <see cref="Entity"/>
        /// in degrees.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// The absolute rotation of the <see cref="Entity"/>
        /// in radians.
        /// </summary>
        public float RotationRadians
        {
            get { return Rotation / 180f * Mathf.PI; }
            set { Rotation = value * 180f / Mathf.PI; }
        }

        /// <summary>
        /// Returns the position relative to the parent <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRelativePosition()
        {
            var parentPosition = Entity.Parent?.GetBehavior<TransformBehavior>()?.Position ?? Vector2.Zero;
            return Position - parentPosition;
        }

        /// <summary>
        /// Returns the scale relative to the parent <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRelativeScale()
        {
            var parentScale = Entity.Parent?.GetBehavior<TransformBehavior>()?.Scale ?? Vector2.One;
            return Scale / parentScale;
        }

        /// <summary>
        /// Returns the rotation relative to the parent <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public float GetRelativeRotation()
        {
            var parentRotation = Entity.Parent?.GetBehavior<TransformBehavior>()?.Rotation ?? 0;
            return Rotation - parentRotation;
        }
    }
}
