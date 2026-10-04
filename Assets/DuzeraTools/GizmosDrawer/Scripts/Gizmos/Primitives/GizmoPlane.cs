using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a thin plane gizmo in wireframe or filled mode.")]
public class GizmoPlane : Gizmo
{
#if UNITY_EDITOR
    // Draws a thin square plane centered on the transform position.
    // Caveat: when wireframe is disabled, this script still draws wireframe outlines before the filled plane.
    [Header("Plane Settings")]
    // Draw mode toggle for wireframe vs filled rendering.
    [SerializeField] private bool _wireframe = false;

    protected override void DrawGizmo()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        if (_wireframe)
            GizmoUtils.DrawWireCube(transform, Vector3.zero, new Vector3(Size, 0.01f, Size), _color, _thickness);
        else
        {
            GizmoUtils.DrawWireCube(transform, Vector3.zero, new Vector3(Size, 0.01f, Size), _color, _thickness);
            Gizmos.DrawCube(Vector3.zero, new Vector3(Size, 0.01f, Size));
        }
    }
#endif
}
}
