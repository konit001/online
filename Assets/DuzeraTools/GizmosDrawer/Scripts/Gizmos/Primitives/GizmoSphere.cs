using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a sphere gizmo in wireframe or filled mode.")]
public class GizmoSphere : Gizmo
{
#if UNITY_EDITOR
    // Draws a sphere gizmo as wireframe-only or as a filled sphere.
    [Header("Sphere Settings")]
    // Draw mode toggle for wireframe vs filled rendering.
    [SerializeField] private bool _wireframe = true;

    protected override void DrawGizmo()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        if (_wireframe)
            GizmoUtils.DrawWireSphere(transform, Vector3.zero, Size, _color, _thickness);
        else
            Gizmos.DrawSphere(Vector3.zero, Size);
    }
#endif
}
}
