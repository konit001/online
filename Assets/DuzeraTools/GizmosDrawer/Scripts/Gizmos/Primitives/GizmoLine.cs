using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a thin line gizmo.")]
public class GizmoLine : Gizmo
{
#if UNITY_EDITOR
    // Draws a thin line centered on the transform position.

    protected override void DrawGizmo()
    {
        Handles.matrix = transform.localToWorldMatrix;

        Handles.color = _color;
        Handles.DrawLine(Vector3.back * Size, Vector3.forward * Size, _thickness);
    }
#endif
}
}
