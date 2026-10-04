using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Displays local X/Y/Z axes with customizable colors and optional labels.")]
public class GizmoAxis : Gizmo
{
#if UNITY_EDITOR
    // Draws local axis arrows (X/Y/Z) and optional axis labels.
    [Header("Axis Settings")]
    // Toggles X, Y, Z label text near axis tips.
    [SerializeField] private bool _showLabels = true;
    // Mirrors the axis.
    [SerializeField] private bool _mirror = false;

    [Header("Axis Colors")]
    // Color used to render the X axis.
    [SerializeField] private Color _xColor = new(1f, 0.2f, 0.2f, 1f);
    // Color used to render the Y axis.
    [SerializeField] private Color _yColor = new(0.2f, 1f, 0.2f, 1f);
    // Color used to render the Z axis.
    [SerializeField] private Color _zColor = new(0.2f, 0.4f, 1f, 1f);

    protected override void DrawGizmo()
    {
        if (ShowColorField)
            ShowColorField = false;

        DrawAxis(Vector3.right, _xColor, "X");
        DrawAxis(Vector3.up, _yColor, "Y");
        DrawAxis(Vector3.forward, _zColor, "Z");

        if (!_mirror)
            return;

        DrawAxis(Vector3.left, _xColor, "X");
        DrawAxis(Vector3.down, _yColor, "Y");
        DrawAxis(Vector3.back, _zColor, "Z");
    }

    private void DrawAxis(Vector3 direction, Color color, string label)
    {
        Vector3 origin = Vector3.zero;
        float length = Size;
        Vector3 end = origin + direction * length;

        GizmoUtils.DrawConicArrow(transform, origin, direction, length, color, _thickness);

        if (_showLabels)
            GizmoUtils.DrawLabel(end + direction * 0.25f, label, color, TextAnchor.LowerCenter);
    }
#endif
}
}
