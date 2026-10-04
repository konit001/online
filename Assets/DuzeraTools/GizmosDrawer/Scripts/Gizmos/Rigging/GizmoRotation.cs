using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws the possible rotation angles of a joint, helping to visualize the range of motion for rigging and animation.")]
public class GizmoRotation : Gizmo
{
#if UNITY_EDITOR
    // Draws a circular arc around the transform position to represent rotation limits.
    [Header("Arc Settings")]
    // Rotation limits in degrees around the local up axis.
    [SerializeField, MinMaxSlider(-180f, 180f)] private Vector2 _rotationLimits = new(-90f, 90f);
    [SerializeField] private FloatReference _referencedMinAngle;
    [SerializeField] private FloatReference _referencedMaxAngle;
    [Space]
    // When enabled, draws radius lines to close the arc at start and end points.
    [SerializeField] private bool _closedArc = true;

    private float MinAngle => HasMinReference ? _referencedMinAngle.GetValue() : _rotationLimits.x;
    private float MaxAngle => HasMaxReference ? _referencedMaxAngle.GetValue() : _rotationLimits.y;
    private bool HasMinReference => _referencedMinAngle != null && _referencedMinAngle._target != null;
    private bool HasMaxReference => _referencedMaxAngle != null && _referencedMaxAngle._target != null;

    protected override void DrawGizmo()
    {
        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;

        // Mirror limits so positive/negative values are easier to reason about visually.
        float startLimit = Mathf.Min(-MinAngle, -MaxAngle);
        float endLimit = Mathf.Max(-MinAngle, -MaxAngle);
        float arcAngle = endLimit - startLimit;

        if (arcAngle <= 0f)
            return;

        float radius = Mathf.Max(0.01f, Size * 0.5f);
        Vector3 normal = Vector3.up;
        Vector3 baseDirection = Vector3.forward;

        // Handles doesn't need values above 360; treat oversized ranges as a full circle.
        if (arcAngle >= 360f)
            arcAngle = 360f;

        Vector3 startDirection = Quaternion.AngleAxis(startLimit, normal) * baseDirection;
        Vector3 endDirection = Quaternion.AngleAxis(endLimit, normal) * baseDirection;

        Handles.DrawWireArc(Vector3.zero, normal, startDirection, arcAngle, radius, _thickness);

        if (_closedArc && arcAngle < 360f)
        {
            Handles.DrawLine(Vector3.zero, startDirection * radius, _thickness);
            Handles.DrawLine(Vector3.zero, endDirection * radius, _thickness);
        }
    }
#endif
}
}
