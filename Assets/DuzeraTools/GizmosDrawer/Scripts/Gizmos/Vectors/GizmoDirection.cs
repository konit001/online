using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws direction toward a target and optional Euler/Quaternion label.")]
public class GizmoDirection : Gizmo
{
#if UNITY_EDITOR
    // Draws an arrow toward a target transform and can display rotation values.
    [Header("Direction Settings")]
    // Target transform used to compute direction from this object.
    [SerializeField] private Transform _endTransform;
    // Toggles rotation text label display.
    [SerializeField] private bool _showLabels = true;
    // Shows rotation as quaternion instead of Euler angles in the label.
    [SerializeField] private bool _showAsQuaternion = false;

    protected override void DrawGizmo()
    {
        if (_endTransform == null)
            return;

        Vector3 origin = transform.position;
        Vector3 direction = _endTransform.position - origin;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        float length = Mathf.Max(0.01f, Size);
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 side = Vector3.Cross(direction, Vector3.up);

        if (side.sqrMagnitude < 0.0001f)
            side = Vector3.Cross(direction, Vector3.right);

        GizmoUtils.DrawArrowShape(transform, origin, direction, side.normalized, length, _color, thickness: _thickness, local: false);

        if (!_showLabels)
            return;

        string label = _showAsQuaternion
            ? $"({rotation.x:F2}, {rotation.y:F2}, {rotation.z:F2}, {rotation.w:F2})"
            : $"({rotation.eulerAngles.x:F1} deg, {rotation.eulerAngles.y:F1} deg, {rotation.eulerAngles.z:F1} deg)";

        GizmoUtils.DrawLabel(origin, label, _color, TextAnchor.LowerCenter);
    }

    public override void SetTargets(Transform[] transforms)
    {
        if (transforms == null || transforms.Length < 1)
            return;

        _endTransform = transforms[0];
    }
#endif
}
}
