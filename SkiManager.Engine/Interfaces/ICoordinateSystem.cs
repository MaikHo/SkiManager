using System.Numerics;
using Windows.Foundation;

namespace SkiManager.Engine.Interfaces
{
    interface ICoordinateSystem
    {
        Vector2 TransformToDips(Vector2 worldPosition);
        Vector2 TransformToDips(Vector3 worldPosition);
        Rect TransformToDips(Rect worldRect);
        Vector3 TransformToWorld(Vector2 dipsPosition);
        Rect TransformToWorld(Rect dipsRect);
        float SampleHeight(Vector2 worldPosition);
    }
}
