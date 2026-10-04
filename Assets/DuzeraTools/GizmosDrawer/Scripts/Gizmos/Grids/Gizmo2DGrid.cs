using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a 2D grid on the transform plane with alignment, points, and cell index options.")]
public class Gizmo2DGrid : Gizmo
{
#if UNITY_EDITOR
    // Draws a 2D grid on the transform right/forward plane.
    [Header("Grid Settings")]
    // Cell count across transform.right.
    [SerializeField] private int _width = 4;
    // Optional external source for width cell count.
    [SerializeField] private FloatReference _referencedWidth;
    // Cell count across transform.forward (depth/Z span).
    [SerializeField] private int _depth = 4;
    // Optional external source for depth cell count.
    [SerializeField] private FloatReference _referencedDepth;
    [Space]
    // Draws a small sphere at every grid intersection.
    [SerializeField] private bool _showGridPoints = false;
    // Draws cell coordinates at each cell center.
    [SerializeField] private bool _showGridIndexes = false;

    [Header("Grid Alignment")]
    // Horizontal pivot placement relative to total grid width.
    [SerializeField] private AlignmentHorizontal _horizontalAlignment = AlignmentHorizontal.Center;
    // Forward/depth pivot placement relative to total grid depth.
    [SerializeField] private AlignmentDepth _depthAlignment = AlignmentDepth.Center;

    protected int Width => HasWidthReference ? (int)_referencedWidth.GetValue() : _width;
    private bool HasWidthReference => _referencedWidth != null && _referencedWidth._target != null;

    protected int Depth => HasDepthReference ? (int)_referencedDepth.GetValue() : _depth;
    private bool HasDepthReference => _referencedDepth != null && _referencedDepth._target != null;

    protected override void DrawGizmo()
    {
        int width = Mathf.Max(1, Width);
        int depth = Mathf.Max(1, Depth);
        float cellSize = Mathf.Max(0.01f, Size);

        Vector3 right = Vector3.right;
        Vector3 forward = Vector3.forward;
        Vector3 origin = Vector3.zero;

        float widthSpan = width * cellSize;
        float depthSpan = depth * cellSize;

        float horizontalOffset = _horizontalAlignment switch
        {
            AlignmentHorizontal.Left => 0f,
            AlignmentHorizontal.Center => -widthSpan * 0.5f,
            AlignmentHorizontal.Right => -widthSpan,
            _ => -widthSpan * 0.5f
        };

        float depthOffset = _depthAlignment switch
        {
            AlignmentDepth.Back => 0f,
            AlignmentDepth.Center => -depthSpan * 0.5f,
            AlignmentDepth.Front => -depthSpan,
            _ => -depthSpan * 0.5f
        };

        Vector3 start =
            origin +
            (right * horizontalOffset) +
            (forward * depthOffset);

        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;

        for (int x = 0; x <= width; x++)
        {
            Vector3 lineStart = start + (right * (x * cellSize));
            Vector3 lineEnd = lineStart + (forward * depthSpan);
            Handles.DrawLine(lineStart, lineEnd, _thickness);
        }

        for (int z = 0; z <= depth; z++)
        {
            Vector3 lineStart = start + (forward * (z * cellSize));
            Vector3 lineEnd = lineStart + (right * widthSpan);
            Handles.DrawLine(lineStart, lineEnd, _thickness);
        }

        if (_showGridPoints)
        {
            float sphereSize = cellSize * 0.15f;
            for (int x = 0; x <= width; x++)
            {
                for (int z = 0; z <= depth; z++)
                {
                    Vector3 pointPosition =
                        start +
                        (right * (x * cellSize)) +
                        (forward * (z * cellSize));

                    Handles.SphereHandleCap(0, pointPosition, Quaternion.identity, sphereSize, EventType.Repaint);
                }
            }
        }

        if (!_showGridIndexes)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 cellCenter =
                    start +
                    (right * ((x + 0.5f) * cellSize)) +
                    (forward * ((z + 0.5f) * cellSize));

                GizmoUtils.DrawLabel(cellCenter, $"{x},{z}", _color, TextAnchor.MiddleCenter);
            }
        }
    }
#endif
}
}
