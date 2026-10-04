using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Displays the angle between two targets as an arc and degree label.")]
public class GizmoAngle : Gizmo
{
#if UNITY_EDITOR
    // Draws the angle arc between two target directions from this transform.
    [Header("Angle Settings")]
    // First transform used to define the first direction vector.
    [SerializeField] private Transform _startTransform;
    // Second transform used to define the second direction vector.
    [SerializeField] private Transform _endTransform;
    // If true, only arc lines are drawn (no solid fill).
    [SerializeField] private bool _wireframe = false;

    protected override void DrawGizmo()
    {
        if (_startTransform == null || _endTransform == null)
            return;

        Vector3 origin = transform.position;
        Vector3 dirA = _startTransform.position - origin;
        Vector3 dirB = _endTransform.position - origin;

        if (dirA.sqrMagnitude < 0.0001f || dirB.sqrMagnitude < 0.0001f)
            return;

        dirA.Normalize();
        dirB.Normalize();

        float angle = Vector3.Angle(dirA, dirB);
        float radius = Mathf.Max(0.01f, Size);

        Vector3 normal = Vector3.Cross(dirA, dirB).normalized;
        if (normal.sqrMagnitude < 0.0001f)
            return;

        Color fillColor = _color;
        fillColor.a *= 0.25f;

        if (!_wireframe)
        {
            Handles.color = fillColor;
            Handles.DrawSolidArc(origin, normal, dirA, angle, radius);
        }

        Handles.color = _color;
        Handles.DrawWireArc(origin, normal, dirA, angle, radius, _thickness);
        Handles.DrawLine(origin, origin + dirA * radius, _thickness);
        Handles.DrawLine(origin, origin + dirB * radius, _thickness);

        Quaternion midRot = Quaternion.AngleAxis(angle * 0.5f, normal);
        Vector3 labelPos = origin + (midRot * dirA) * (radius * 1.15f);

        GizmoUtils.DrawLabel(labelPos, $"{angle:F1}°", _color, TextAnchor.LowerCenter);
    }

    public override void SetTargets(Transform[] transforms)
    {
        if (transforms == null || transforms.Length < 2)
            return;

        _startTransform = transforms[0];
        _endTransform = transforms[1];
    }
#endif
}
}
