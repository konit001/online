using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws an arrow aligned to the transform forward/right orientation.")]
public class GizmoArrowSingle : Gizmo
{
#if UNITY_EDITOR
    // Draws an arrow shape oriented by the transform forward/right vectors.

    [Header("Arrow Settings")]
    // Draw mode toggle for wireframe vs filled rendering.
    [SerializeField] private bool _wireframe = true;
    protected override void DrawGizmo()
    {
        Vector3 forward = Vector3.forward.normalized;

        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;

        Vector3 direction = Vector3.right.normalized;

        GizmoUtils.DrawArrowShape(transform, Vector3.zero, forward, direction, Size, _color, filled: _wireframe, thickness: _thickness);
    }
#endif
}
}
