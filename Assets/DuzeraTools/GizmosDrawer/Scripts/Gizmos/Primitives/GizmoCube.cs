using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a cube gizmo in wireframe or filled mode.")]
public class GizmoCube : Gizmo
{
#if UNITY_EDITOR
    // Draws a cube gizmo as wireframe-only or as a filled cube.
    [Header("Cube Settings")]
    // Draw mode toggle for wireframe vs filled rendering.
    [SerializeField] private bool _wireframe = true;

    protected override void DrawGizmo()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        if (_wireframe)
            GizmoUtils.DrawWireCube(transform, Vector3.zero, Vector3.one * Size, _color, _thickness);
        else
        {
            GizmoUtils.DrawWireCube(transform, Vector3.zero, Vector3.one * Size, _color, _thickness);
            Gizmos.DrawCube(Vector3.zero, Vector3.one * Size);
        }
    }
#endif
}
}
