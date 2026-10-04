using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Visualizes a waypoint path as linear or smoothed spline segments.")]
public class GizmoPath : Gizmo
{
#if UNITY_EDITOR
    // Draws a waypoint path as straight segments or a smoothed spline.

    [Header("Path Settings")]
    // Ordered waypoint list used to build the path.
    [SerializeField] private Transform[] _waypoints = new Transform[2];
    // Uses smoothed Catmull-Rom interpolation when enabled.
    [SerializeField] private bool _useCatmullRom = false;
    // Draws segment and total distance labels.
    [SerializeField] private bool _showDistances = true;

    protected override void DrawGizmo()
    {
        if (_waypoints == null || _waypoints.Length < 2)
            return;

        if (_waypoints[0] == null)
            _waypoints[0] = transform;

        if (_waypoints[1] == null)
            return;

        Vector3[] worldPoints = new Vector3[_waypoints.Length];
        for (int i = 0; i < _waypoints.Length; i++)
            worldPoints[i] = _waypoints[i].position;

        float totalDistance = 0f;
        float[] segmentDistances = new float[worldPoints.Length - 1];

        for (int i = 0; i < worldPoints.Length - 1; i++)
        {
            if (!_useCatmullRom)
                segmentDistances[i] = Vector3.Distance(worldPoints[i], worldPoints[i + 1]);
            else
                segmentDistances[i] = CalculateCatmullRomDistance(worldPoints, i);
            totalDistance += segmentDistances[i];
        }

        Handles.color = _color;

        if (!_useCatmullRom)
            DrawLinearPath(worldPoints);
        else
            DrawCatmullRomPath(worldPoints);

        for (int i = 0; i < worldPoints.Length; i++)
        {
            Handles.color = i == 0 ? Color.green : (i == worldPoints.Length - 1 ? Color.red : Color.white);
            Gizmos.DrawWireSphere(worldPoints[i], Size * 0.2f);
        }

        if (_showDistances)
        {
            for (int i = 0; i < worldPoints.Length - 1; i++)
            {
                Vector3 midpoint = (worldPoints[i] + worldPoints[i + 1]) * 0.5f;
                GizmoUtils.DrawLabel(midpoint, $"D: {segmentDistances[i]:F2}", _color, TextAnchor.LowerCenter);
            }

            GizmoUtils.DrawLabel(worldPoints[0], $"Total: {totalDistance:F2}", _color, TextAnchor.LowerCenter);
        }
    }

    private void DrawLinearPath(Vector3[] points)
    {
        Handles.color = _color;
        for (int i = 0; i < points.Length - 1; i++)
            Handles.DrawLine(points[i], points[i + 1], _thickness);
    }

    private void DrawCatmullRomPath(Vector3[] points)
    {
        Handles.color = _color;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];

            Vector3 prevPoint = p0;
            for (int j = 1; j <= 16; j++)
            {
                float t = j / 16f;
                Vector3 curvePoint = CatmullRom(
                    i > 0 ? points[i - 1] : p0,
                    p0,
                    p1,
                    i < points.Length - 2 ? points[i + 2] : p1,
                    t
                );
                Handles.DrawLine(prevPoint, curvePoint, _thickness);
                prevPoint = curvePoint;
            }
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private float CalculateCatmullRomDistance(Vector3[] points, int segmentIndex)
    {
        Vector3 p0 = points[segmentIndex];
        Vector3 p1 = points[segmentIndex + 1];
        float distance = 0f;
        Vector3 prevPoint = p0;
        for (int i = 1; i <= 16; i++)
        {
            float t = i / 16f;
            Vector3 curvePoint = CatmullRom(
                segmentIndex > 0 ? points[segmentIndex - 1] : p0,
                p0,
                p1,
                segmentIndex < points.Length - 2 ? points[segmentIndex + 2] : p1,
                t
            );
            distance += Vector3.Distance(prevPoint, curvePoint);
            prevPoint = curvePoint;
        }
        return distance;
    }

    public override void SetTargets(Transform[] transforms)
    {
        if (transforms == null || transforms.Length < 2)
            return;

        _waypoints = new Transform[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            _waypoints[i] = transforms[i];
    }
#endif
}
}
