using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Measures and displays distance between start and target transforms.\n if start is null, uses this transform position as default.")]
public class GizmoDistance : Gizmo
{
#if UNITY_EDITOR
    // Draws a line between two points and labels the measured distance.
    // Caveat: when start transform is null, the script silently uses this transform position.
    [Header("Distance Settings")]
    // Optional custom start point for measurement.
    [SerializeField] private Transform _startTransform;
    // Required target/end point for measurement.
    [SerializeField] private Transform _targetTransform;

    protected override void DrawGizmo()
    {
        if (_targetTransform == null)
            return;

        Vector3 worldPointA = _startTransform != null ? _startTransform.position : transform.position;
        Vector3 worldPointB = _targetTransform.position;

        float distance = Vector3.Distance(worldPointA, worldPointB);

        Handles.color = _color;
        Handles.DrawLine(worldPointA, worldPointB, _thickness);

        Gizmos.DrawWireSphere(worldPointA, Size * 0.2f);
        Gizmos.DrawWireSphere(worldPointB, Size * 0.2f);

        Vector3 midpoint = (worldPointA + worldPointB) * 0.5f;
        Handles.color = _color;

        GizmoUtils.DrawLabel(midpoint, $"Distance: {distance:F2}", _color, TextAnchor.LowerCenter);
    }

    public override void SetTargets(Transform[] transforms)
    {
        if (transforms == null || transforms.Length < 2)
            return;

        _startTransform = transforms[0];
        _targetTransform = transforms[1];
    }
#endif
}
}
