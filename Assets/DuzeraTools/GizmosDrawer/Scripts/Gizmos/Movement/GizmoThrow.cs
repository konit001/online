using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a throw trajectory gizmo.")]
public class GizmoThrow : Gizmo
{
#if UNITY_EDITOR
    // Draws a throw trajectory line using this transform's forward direction.
    private const int Steps = 16;

    [Header("Throw Settings")]
    // Local fallback launch speed applied along transform.forward.
    [SerializeField, Min(0f)] private float _strength = 5f;
    // Optional external source for launch strength.
    [SerializeField] private FloatReference _referencedStrength;
    // Local fallback downward acceleration used to bend the arc.
    [SerializeField, Min(0f)] private float _gravity = 9.81f;
    // Optional external source for gravity.
    [SerializeField] private FloatReference _referencedGravity;
    // Local fallback amount of simulated time to preview.
    [SerializeField, Min(0.05f)] private float _duration = 1.5f;
    // Optional external source for duration.
    [SerializeField] private FloatReference _referencedDuration;
    // Draws a short arrow showing the launch direction.
    [SerializeField] private bool _drawLaunchArrow = true;
    // Draws strength and gravity text at the end of the preview.
    [SerializeField] private bool _showLabels = true;

    [Header("Collision")]
    // Raycasts each trajectory segment and stops the preview at the first hit.
    [SerializeField] private bool _stopAtCollider = false;
    // Layers that the trajectory collision test can hit.
    [SerializeField] private LayerMask _collisionLayerMask = ~0;

    private float Strength => HasStrengthReference ? _referencedStrength.GetValue() : _strength;
    private float Gravity => HasGravityReference ? _referencedGravity.GetValue() : _gravity;
    private float Duration => HasDurationReference ? _referencedDuration.GetValue() : _duration;

    private bool HasStrengthReference => _referencedStrength != null && _referencedStrength._target != null;
    private bool HasGravityReference => _referencedGravity != null && _referencedGravity._target != null;
    private bool HasDurationReference => _referencedDuration != null && _referencedDuration._target != null;

    protected override void DrawGizmo()
    {
        Handles.matrix = Matrix4x4.identity;
        Handles.color = _color;

        Vector3 startPosition = transform.position;
        float strength = Mathf.Max(0f, Strength);
        float gravity = Mathf.Max(0f, Gravity);
        float duration = Mathf.Max(0.05f, Duration);
        Vector3 launchVelocity = transform.forward * strength;
        Vector3 gravityAcceleration = Vector3.down * gravity;

        Vector3[] points = new Vector3[Steps + 1];

        for (int i = 0; i < points.Length; i++)
        {
            float normalizedTime = i / (float)Steps;
            float time = normalizedTime * duration;
            points[i] = startPosition + (launchVelocity * time) + (0.5f * gravityAcceleration * time * time);
        }

        Vector3 finalPoint = _stopAtCollider
            ? GizmoUtils.DrawRaycastPath(transform, points, _collisionLayerMask, _color, _thickness)
            : points[points.Length - 1];

        if (!_stopAtCollider)
            Handles.DrawAAPolyLine(_thickness, points);

        if (_drawLaunchArrow && strength > 0f)
        {
            float arrowLength = Mathf.Max(0.01f, Size);
            Vector3 arrowTip = startPosition + (transform.forward * arrowLength);

            Handles.DrawLine(startPosition, arrowTip, _thickness);
            Handles.ConeHandleCap(0, arrowTip, Quaternion.LookRotation(transform.forward), arrowLength * 0.2f, EventType.Repaint);
        }

        if (_showLabels)
        {
            GizmoUtils.DrawLabel(finalPoint, $"Strength {strength:F1} | Gravity {gravity:F1}", _color, TextAnchor.LowerCenter);
            Handles.matrix = Matrix4x4.identity;
        }
    }
#endif
}
}
