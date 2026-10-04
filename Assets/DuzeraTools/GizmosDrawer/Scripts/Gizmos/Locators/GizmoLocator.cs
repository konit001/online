using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Displays a 3-axis crosshair locator at the transform position.")]
public class GizmoLocator : Gizmo
{
#if UNITY_EDITOR
    // Draws a 3D crosshair locator centered at the transform position.
    protected override void DrawGizmo()
    {
        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;

        Handles.DrawLine(Vector3.forward * (Size * 0.5f), -Vector3.forward * (Size * 0.5f), _thickness);
        Handles.DrawLine(Vector3.right * (Size * 0.5f), -Vector3.right * (Size * 0.5f), _thickness);
        Handles.DrawLine(Vector3.up * (Size * 0.5f), -Vector3.up * (Size * 0.5f), _thickness);
    }
#endif
}
}
