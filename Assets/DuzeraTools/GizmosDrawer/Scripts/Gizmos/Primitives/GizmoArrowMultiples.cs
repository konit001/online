using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws multiple arrows aligned to the transform forward/right orientation.")]
public class GizmoArrowMultiples : Gizmo
{
#if UNITY_EDITOR
    [Header("Multiple Arrow Settings")]
    [SerializeField, Range(2, 8)] private int _arrowCount = 4;

    // Draws arrows distributed radially and connects their tips.
    protected override void DrawGizmo()
    {
        Vector3 center = Vector3.zero;
        Vector3 axis = Vector3.up;
        if (axis.sqrMagnitude < 0.0001f)
            axis = Vector3.up;

        Vector3 firstDirection = Vector3.forward;
        if (firstDirection.sqrMagnitude < 0.0001f)
            firstDirection = Vector3.forward;

        firstDirection = Vector3.ProjectOnPlane(firstDirection, axis).normalized;
        if (firstDirection.sqrMagnitude < 0.0001f)
            firstDirection = Vector3.Cross(axis, Vector3.right).normalized;
        if (firstDirection.sqrMagnitude < 0.0001f)
            firstDirection = Vector3.Cross(axis, Vector3.forward).normalized;

        int count = Mathf.Clamp(_arrowCount, 2, 8);
        float step = 360f / count;

        Vector3[] arrowTips = new Vector3[count];
        float arrowLength = Mathf.Max(0.01f, Size);
        float offsetDistance = Mathf.Max(0.01f, 0.05f * (count - 1) * Size);

        for (int i = 0; i < count; i++)
        {
            Vector3 radialDirection = Quaternion.AngleAxis(step * i, axis) * firstDirection;
            Vector3 arrowStart = center + radialDirection * offsetDistance;

            Vector3 side = Vector3.Cross(radialDirection, axis).normalized;
            if (side.sqrMagnitude < 0.0001f)
                side = transform.right.normalized;

            GizmoUtils.DrawArrowShape(transform, arrowStart, radialDirection, side, arrowLength, _color, thickness: _thickness, filled: false, closed: false);
            arrowTips[i] = arrowStart + radialDirection * (arrowLength * 0.65f);
        }
    }
#endif
}
}
