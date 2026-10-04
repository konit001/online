using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Visualizes a field-of-view sector with optional occlusion clipping against colliders.")]
public class GizmoFOV : Gizmo
{
#if UNITY_EDITOR
    // Draws a field-of-view sector, optionally clipped by world colliders.
    [Header("FOV Settings")]
    // Local fallback FOV angle in degrees when no FloatReference is assigned.
    [SerializeField, Range(1f, 360f)] private float _angle = 60f;
    // Optional external source for the FOV angle.
    [SerializeField] private FloatReference _referencedAngle;
    // If true, only boundary lines are drawn (no filled wedges).
    [SerializeField] private bool _wireframe = false;
    // Inner cutoff distance from origin where the FOV starts.
    [SerializeField, Min(0f)] private float _minDistance = 0f;
    // Optional external source for the inner cutoff distance.
    [SerializeField] private FloatReference _referencedMinDistance;

    [Header("Occlusion")]
    // If true, each FOV ray stops at the first collider hit.
    [SerializeField] private bool _stopAtCollider = true;
    // Layers considered as obstacles when clipping rays.
    [SerializeField] private LayerMask _obstacleMask = ~0;
    // Includes trigger colliders in occlusion tests when enabled.
    [SerializeField] private bool _includeTriggers = false;

    private float Angle => HasReference ? _referencedAngle.GetValue() : _angle;
    private bool HasReference => _referencedAngle != null && _referencedAngle._target != null;

    private float MinDistance => HasMinDistanceReference ? _referencedMinDistance.GetValue() : _minDistance;
    private bool HasMinDistanceReference => _referencedMinDistance != null && _referencedMinDistance._target != null;

    protected override void DrawGizmo()
    {
        Vector3 origin = transform.position;
        Vector3 planarForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        if (planarForward.sqrMagnitude < 0.0001f)
            planarForward = Vector3.forward;

        float radius = Mathf.Max(0.01f, Size);
        float minDist = Mathf.Clamp(MinDistance, 0f, radius - 0.001f);
        float clampedAngle = Mathf.Clamp(Angle, 1f, 360f);
        float halfAngle = clampedAngle * 0.5f;
        int rayCount = 32;

        Vector3[] innerPoints = new Vector3[rayCount + 1];
        Vector3[] outerPoints = new Vector3[rayCount + 1];
        QueryTriggerInteraction queryTriggers = _includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = i / (float)rayCount;
            float rayAngle = -halfAngle + (clampedAngle * t);
            Vector3 dir = Quaternion.AngleAxis(rayAngle, Vector3.up) * planarForward;

            float outerDist = radius;
            if (_stopAtCollider && Physics.Raycast(origin, dir, out RaycastHit hit, radius, _obstacleMask, queryTriggers))
            {
                if (hit.distance > minDist)
                    outerDist = hit.distance;
            }

            innerPoints[i] = origin + dir * minDist;
            outerPoints[i] = origin + dir * outerDist;
        }

        Color fillColor = _color;
        fillColor.a *= 0.25f;

        if (!_wireframe)
        {
            Handles.color = fillColor;
            for (int i = 0; i < rayCount; i++)
                Handles.DrawAAConvexPolygon(innerPoints[i], innerPoints[i + 1], outerPoints[i + 1], outerPoints[i]);
        }

        Handles.color = _color;
        if (clampedAngle < 359.9f)
        {
            Handles.DrawLine(innerPoints[0], outerPoints[0], _thickness);
            Handles.DrawLine(innerPoints[rayCount], outerPoints[rayCount], _thickness);
        }
    }
#endif
}
}
