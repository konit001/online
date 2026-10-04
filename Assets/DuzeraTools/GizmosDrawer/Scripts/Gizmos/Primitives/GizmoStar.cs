using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a star gizmo in wireframe or filled mode.")]
public class GizmoStar : Gizmo
{
#if UNITY_EDITOR
    // Draws a four-point star gizmo as wireframe-only or as a filled star.
    [Header("Star Settings")]
    // Draw mode toggle for wireframe vs filled rendering.
    [SerializeField] private bool _wireframe = true;
    
    private readonly float _innerRadiusRatio = 0.35f;

    protected override void DrawGizmo()
    {
        Vector3 center = Vector3.zero;
        float outerRadius = Mathf.Max(0.01f, Size * 0.5f);
        float innerRadius = outerRadius * _innerRadiusRatio;

        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;

        Handles.matrix = transform.localToWorldMatrix;

        Vector3[] points = new Vector3[8];
        for (int i = 0; i < points.Length; i++)
        {
            float angle = i * 45f;
            float radius = (i % 2 == 0) ? outerRadius : innerRadius;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = (Mathf.Cos(rad) * up) + (Mathf.Sin(rad) * right);
            points[i] = center + dir * radius;
        }

        if (!_wireframe)
        {
            Color fillColor = new(_color.r, _color.g, _color.b, _color.a * 0.35f);
            Handles.color = fillColor;

            for (int i = 0; i < points.Length; i++)
            {
                int next = (i + 1) % points.Length;
                Handles.DrawAAConvexPolygon(center, points[i], points[next]);
            }
        }

        Handles.color = _color;
        Vector3[] linePoints = new Vector3[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
            linePoints[i] = points[i];

        linePoints[linePoints.Length - 1] = points[0];
        Handles.DrawAAPolyLine(_thickness, linePoints);
    }
#endif
}
}
