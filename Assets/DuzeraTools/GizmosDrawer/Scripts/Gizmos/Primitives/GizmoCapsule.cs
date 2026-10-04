using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a capsule gizmo with configurable axis, radius, and height.")]
public class GizmoCapsule : Gizmo
{
#if UNITY_EDITOR
    public enum CapsuleDirection { X = 0, Y = 1, Z = 2 }

    // Draws a wire capsule with configurable orientation and dimensions.
    [Header("Capsule Settings")]
    // Axis used as the capsule's length direction.
    [SerializeField] private CapsuleDirection _direction = CapsuleDirection.Y;
    // Radius of both spherical ends and cylinder body.
    [SerializeField, Min(0.01f)] private float _radius = 0.5f;
    // Total capsule height along the selected axis.
    [SerializeField, Min(0.01f)] private float _height = 2f;

    protected override void DrawGizmo()
    {
        Vector3 origin = Vector3.zero;
        float radius = _radius;
        float height = _height;

        Vector3 axisDir = _direction switch
        {
            CapsuleDirection.X => Vector3.right,
            CapsuleDirection.Y => Vector3.up,
            CapsuleDirection.Z => Vector3.forward,
            _ => Vector3.up
        };

        if (axisDir.sqrMagnitude < 0.0001f)
            axisDir = Vector3.up;

        Vector3 perp1 = Vector3.Cross(axisDir, Mathf.Abs(Vector3.Dot(axisDir, Vector3.forward)) > 0.9f ? Vector3.right : Vector3.forward).normalized;
        Vector3 perp2 = Vector3.Cross(axisDir, perp1).normalized;

        float cylinderHeight = Mathf.Max(0.01f, height - 2f * radius);
        Vector3 topCenter = origin + axisDir * (cylinderHeight * 0.5f);
        Vector3 bottomCenter = origin - axisDir * (cylinderHeight * 0.5f);

        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;

        DrawHemisphere(topCenter, radius, axisDir, perp1, perp2, false);
        DrawHemisphere(bottomCenter, radius, axisDir, perp1, perp2, true);

        for (int i = 0; i < 4; i++)
        {
            float angle = (i / 4f) * Mathf.PI * 2f;
            Vector3 offset = (Mathf.Cos(angle) * perp1 + Mathf.Sin(angle) * perp2) * radius;
            Handles.DrawLine(topCenter + offset, bottomCenter + offset, _thickness);
        }

        Handles.DrawWireDisc(topCenter, axisDir, radius, _thickness);
        Handles.DrawWireDisc(bottomCenter, axisDir, radius, _thickness);
    }

    private void DrawHemisphere(Vector3 center, float radius, Vector3 axisDir, Vector3 perp1, Vector3 perp2, bool isBottom)
    {
        int segments = 4;
        int hemisegments = 8;

        for (int h = 0; h < hemisegments; h++)
        {
            float phi0 = (h / (float)hemisegments) * (Mathf.PI * 0.5f);
            float phi1 = ((h + 1) / (float)hemisegments) * (Mathf.PI * 0.5f);

            if (isBottom)
            {
                phi0 = Mathf.PI - phi0;
                phi1 = Mathf.PI - phi1;
            }

            for (int i = 0; i < segments; i++)
            {
                float theta0 = (i / (float)segments) * Mathf.PI * 2f;

                Vector3 p00 = center + GetSpherePoint(Mathf.Sin(phi0) * Mathf.Cos(theta0), Mathf.Sin(phi0) * Mathf.Sin(theta0), Mathf.Cos(phi0), radius, axisDir, perp1, perp2);
                Vector3 p01 = center + GetSpherePoint(Mathf.Sin(phi1) * Mathf.Cos(theta0), Mathf.Sin(phi1) * Mathf.Sin(theta0), Mathf.Cos(phi1), radius, axisDir, perp1, perp2);

                Handles.matrix = transform.localToWorldMatrix;

                Handles.DrawLine(p00, p01, _thickness);
            }
        }
    }

    private Vector3 GetSpherePoint(float x, float y, float z, float radius, Vector3 axisDir, Vector3 perp1, Vector3 perp2)
    {
        return (x * perp1 + y * perp2 + z * axisDir) * radius;
    }
#endif
}
}
