using System.Numerics;
using Windows.Foundation;

namespace SkiManager.Engine.Interfaces
{
    /// <summary>
    /// Provides methods that transform virtual coordinates
    /// into DIPs and vice versa for rendering purposes.
    /// </summary>
    public interface ICoordinateSystem
    {
        /// <summary>
        /// Virtual world size.
        /// </summary>
        Vector2 Size { get; }

        /// <summary>
        /// Transforms a 2D virtual world position into
        /// canvas coordinates (DIPs).
        /// </summary>
        /// <param name="worldPosition">Position in world space</param>
        /// <returns>Position in canvas space</returns>
        Vector2 TransformToDips(Vector2 worldPosition);

        /// <summary>
        /// Transforms a 3D virtual world position into
        /// canvas coordinates (DIPs).
        /// </summary>
        /// <param name="worldPosition">Position in world space</param>
        /// <returns>Position in canvas space</returns>
        Vector2 TransformToDips(Vector3 worldPosition);

        /// <summary>
        /// Transforms a rectangle in 2D virtual world space
        /// to canvas coordinates (DIPs).
        /// </summary>
        /// <param name="worldRect">Rectangle in world space</param>
        /// <returns>Rectangle in canvas space</returns>
        Rect TransformToDips(Rect worldRect);

        /// <summary>
        /// Transforms a point from canvas coordinates (DIPs)
        /// to 2D virtual world space coordinates.
        /// </summary>
        /// <param name="dipsPosition">Position in canvas space</param>
        /// <returns>Position in world space</returns>
        Vector2 TransformToWorld2D(Vector2 dipsPosition);

        /// <summary>
        /// Transforms a point from canvas coordinates (DIPs)
        /// to 3D virtual world space coordinates.
        /// </summary>
        /// <param name="dipsPosition">Position in canvas space</param>
        /// <returns>Position in world space</returns>
        Vector3 TransformToWorld3D(Vector2 dipsPosition);

        /// <summary>
        /// Transforms a rectangle from canvas coordinates (DIPs)
        /// to 2D virtual world space coordinates.
        /// </summary>
        /// <param name="dipsRect">Rectangle in canvas space</param>
        /// <returns>Rectangle in world space</returns>
        Rect TransformToWorld2D(Rect dipsRect);

        /// <summary>
        /// Transforms a point in 2D virtual world space to
        /// 3D virtual world space.
        /// </summary>
        /// <param name="worldPosition">2D position in world space</param>
        /// <returns>3D position in world space</returns>
        Vector3 Transform2DTo3D(Vector2 worldPosition);
    }
}
