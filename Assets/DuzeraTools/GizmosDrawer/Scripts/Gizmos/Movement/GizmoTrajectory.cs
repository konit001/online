using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a trajectory gizmo.")]
public class GizmoTrajectory : Gizmo
{
#if UNITY_EDITOR
    // Draws a designer-controlled arc using this transform's forward direction.
    private const int Steps = 16;

    [Header("Trajectory Settings")]
    // Local fallback distance to draw forward from this transform.
    [SerializeField, Min(0f)] private float _length = 5f;
    // Optional external source for trajectory length.
    [SerializeField] private FloatReference _referencedLength;
    // Local fallback height added at the middle control point.
    [SerializeField] private float _height = 2f;
    // Optional external source for trajectory height.
    [SerializeField] private FloatReference _referencedHeight;
    // Draws a short arrow showing the trajectory direction.
    [SerializeField] private bool _drawDirectionArrow = true;
    // Draws length and height text at the end of the preview.
    [SerializeField] private bool _showLabels = true;

    [Header("Collision")]
    // Raycasts each trajectory segment and stops the preview at the first hit.
    [SerializeField] private bool _stopAtCollider = false;
    // Layers that the trajectory collision test can hit.
    [SerializeField] private LayerMask _collisionLayerMask = ~0;

    private float Length => HasLengthReference ? _referencedLength.GetValue() : _length;
    private float Height => HasHeightReference ? _referencedHeight.GetValue() : _height;

    private bool HasLengthReference => _referencedLength != null && _referencedLength._target != null;
    private bool HasHeightReference => _referencedHeight != null && _referencedHeight._target != null;

    protected override void DrawGizmo()
    {
        Handles.matrix = Matrix4x4.identity;
        Handles.color = _color;

        Vector3 startPosition = transform.position;
        float length = Mathf.Max(0f, Length);
        float height = Height;
        Vector3 endPosition = startPosition + (transform.forward * length);
        Vector3 controlPosition = Vector3.Lerp(startPosition, endPosition, 0.5f) + (transform.up * height);

        Vector3[] points = new Vector3[Steps + 1];

        for (int i = 0; i < points.Length; i++)
        {
            float t = i / (float)Steps;
            points[i] = GetQuadraticBezierPoint(startPosition, controlPosition, endPosition, t);
        }

        Vector3 finalPoint = _stopAtCollider
            ? GizmoUtils.DrawRaycastPath(transform, points, _collisionLayerMask, _color, _thickness)
            : endPosition;

        if (!_stopAtCollider)
            Handles.DrawAAPolyLine(_thickness, points);

        if (_drawDirectionArrow && length > 0f)
        {
            float arrowLength = Mathf.Min(Mathf.Max(0.01f, Size), length);
            Vector3 arrowTip = startPosition + (transform.forward * arrowLength);

            Handles.DrawLine(startPosition, arrowTip, _thickness);
            Handles.ConeHandleCap(0, arrowTip, Quaternion.LookRotation(transform.forward), arrowLength * 0.2f, EventType.Repaint);
        }

        if (_showLabels)
            GizmoUtils.DrawLabel(finalPoint, $"Length {length:F1} | Height {height:F1}", _color, TextAnchor.LowerCenter);
    }

    private static Vector3 GetQuadraticBezierPoint(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        float inverseT = 1f - t;
        return (inverseT * inverseT * start) + (2f * inverseT * t * control) + (t * t * end);
    }
#endif
}
}
