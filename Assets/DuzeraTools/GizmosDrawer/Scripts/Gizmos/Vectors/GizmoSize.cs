using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws the total renderer/collider bounds of an object, optionally including children.")]
public class GizmoSize : Gizmo
{
#if UNITY_EDITOR
    // Draws a world-space bounds box and labels its total dimensions.
    [Header("Size Settings")]
    // Includes renderers and colliders from child objects when calculating bounds.
    [SerializeField] private bool _includeChildren = true;
    // Includes Renderer bounds in the size calculation.
    [SerializeField] private bool _includeRenderers = true;
    // Includes Collider bounds in the size calculation.
    [SerializeField] private bool _includeColliders = true;
    // Draws the measured X/Y/Z dimensions above the bounds box.
    [SerializeField] private bool _showLabel = true;

    protected override void DrawGizmo()
    {
        if (ShowSizeField)
            ShowSizeField = false;

        if (ShowReferencedSizeField)
            ShowReferencedSizeField = false;

        if (!TryGetBounds(out Bounds bounds))
            return;

        Handles.matrix = Matrix4x4.identity;
        Handles.color = _color;
        DrawWorldBounds(bounds);

        if (!_showLabel)
            return;

        Vector3 labelPosition = bounds.center + Vector3.up * (bounds.extents.y + 0.1f);
        Vector3 size = bounds.size;
        GizmoUtils.DrawLabel(labelPosition, $"Size: {size.x:F2}, {size.y:F2}, {size.z:F2}", _color, TextAnchor.LowerCenter);
    }

    private bool TryGetBounds(out Bounds bounds)
    {
        bounds = default;
        bool hasBounds = false;

        if (_includeRenderers)
        {
            Renderer[] renderers = _includeChildren
                ? GetComponentsInChildren<Renderer>()
                : GetComponents<Renderer>();

            EncapsulateBounds(renderers, ref bounds, ref hasBounds);
        }

        if (_includeColliders)
        {
            Collider[] colliders = _includeChildren
                ? GetComponentsInChildren<Collider>()
                : GetComponents<Collider>();

            EncapsulateBounds(colliders, ref bounds, ref hasBounds);
        }

        return hasBounds;
    }

    private static void EncapsulateBounds(Renderer[] renderers, ref Bounds bounds, ref bool hasBounds)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            EncapsulateBounds(renderers[i].bounds, ref bounds, ref hasBounds);
        }
    }

    private static void EncapsulateBounds(Collider[] colliders, ref Bounds bounds, ref bool hasBounds)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
                continue;

            EncapsulateBounds(colliders[i].bounds, ref bounds, ref hasBounds);
        }
    }

    private static void EncapsulateBounds(Bounds sourceBounds, ref Bounds bounds, ref bool hasBounds)
    {
        if (!hasBounds)
        {
            bounds = sourceBounds;
            hasBounds = true;
            return;
        }

        bounds.Encapsulate(sourceBounds);
    }

    private void DrawWorldBounds(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3 p0 = center + new Vector3(-extents.x, -extents.y, -extents.z);
        Vector3 p1 = center + new Vector3(extents.x, -extents.y, -extents.z);
        Vector3 p2 = center + new Vector3(extents.x, -extents.y, extents.z);
        Vector3 p3 = center + new Vector3(-extents.x, -extents.y, extents.z);
        Vector3 p4 = center + new Vector3(-extents.x, extents.y, -extents.z);
        Vector3 p5 = center + new Vector3(extents.x, extents.y, -extents.z);
        Vector3 p6 = center + new Vector3(extents.x, extents.y, extents.z);
        Vector3 p7 = center + new Vector3(-extents.x, extents.y, extents.z);

        Handles.DrawLine(p0, p1, _thickness);
        Handles.DrawLine(p1, p2, _thickness);
        Handles.DrawLine(p2, p3, _thickness);
        Handles.DrawLine(p3, p0, _thickness);

        Handles.DrawLine(p4, p5, _thickness);
        Handles.DrawLine(p5, p6, _thickness);
        Handles.DrawLine(p6, p7, _thickness);
        Handles.DrawLine(p7, p4, _thickness);

        Handles.DrawLine(p0, p4, _thickness);
        Handles.DrawLine(p1, p5, _thickness);
        Handles.DrawLine(p2, p6, _thickness);
        Handles.DrawLine(p3, p7, _thickness);
    }
#endif
}
}
