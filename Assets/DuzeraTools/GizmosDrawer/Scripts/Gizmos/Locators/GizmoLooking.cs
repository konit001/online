using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Shows facing direction using a disc and forward arrow indicator.")]
public class GizmoLooking : Gizmo
{
#if UNITY_EDITOR
    // Draws a facing indicator using a disc and a forward-pointing arrow.
    protected override void DrawGizmo()
    {
        Vector3 start = Vector3.zero;
        Vector3 forward = Vector3.forward;

        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;
        Handles.DrawWireDisc(start, Vector3.up, Size * 0.5f, _thickness);

        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;

        GizmoUtils.DrawArrowShape(transform, forward * (Size * 0.5f), forward, Vector3.right, Size * 0.9f, _color, thickness: _thickness);
    }
#endif
}
}
