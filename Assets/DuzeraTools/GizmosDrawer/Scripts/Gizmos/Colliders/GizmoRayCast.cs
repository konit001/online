using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a forward raycast, including hit point and normal visualization.")]
public class GizmoRayCast : Gizmo
{
#if UNITY_EDITOR
    // Draws a forward ray and visual feedback for raycast hit information.
    [Header("Raycast Settings")]
    // Layers that the physics raycast can hit.
    [SerializeField] private LayerMask _layerMask = ~0;

    protected override void DrawGizmo()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward.normalized;

        GizmoUtils.DrawRaycast(transform, origin, direction, Size, _layerMask, _color, _thickness);
    }
#endif
}
}
