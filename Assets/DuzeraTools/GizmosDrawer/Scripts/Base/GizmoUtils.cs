using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
public static class GizmoUtils
{
#if UNITY_EDITOR
    public static void DrawLabel(Vector3 position, string text, Color color, TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        GUIStyle guiStyle = new()
        {
            normal = new GUIStyleState { textColor = color },
            fontSize = 12,
            alignment = alignment
        };

        Handles.Label(position, text, guiStyle);
    }

    public static void DrawNormalArrow(Transform transform, Vector3 start, Vector3 normalDirection, float size, Color color
        , float thickness = 1f, bool filled = true, bool closed = true, bool local = true)
    {
        if (normalDirection.sqrMagnitude < 0.0001f)
            return;

        normalDirection.Normalize();

        Vector3 side = Vector3.Cross(normalDirection, Vector3.up);
        if (side.sqrMagnitude < 0.0001f)
            side = Vector3.Cross(normalDirection, Vector3.right);

        DrawArrowShape(transform, start, normalDirection, side.normalized, size, color, 0.35f, thickness, filled, closed, local);
    }

    public static void DrawArrowShape(Transform transform, Vector3 start, Vector3 arrowDirection, Vector3 side, float size, Color color, float tailLength = 0.35f
        , float thickness = 0.15f, bool filled = true, bool closed = true, bool local = true)
    {
        if (arrowDirection.sqrMagnitude < 0.0001f)
            return;

        float halfWidth = size * 0.15f;

        Vector3 shaftEndCenter = start + (arrowDirection * tailLength);
        Vector3 tip = start + (0.65f * size * arrowDirection);

        Vector3 tailLeft = start - (side * halfWidth);
        Vector3 tailRight = start + (side * halfWidth);
        Vector3 shaftLeft = shaftEndCenter - (side * halfWidth);
        Vector3 shaftRight = shaftEndCenter + (side * halfWidth);
        Vector3 headLeft = shaftEndCenter - (side * (halfWidth * 1.8f));
        Vector3 headRight = shaftEndCenter + (side * (halfWidth * 1.8f));

        Vector3[] points = closed
            ? new[] { shaftLeft, tailLeft, tailRight, shaftRight, headRight, tip, headLeft, shaftLeft }
            : new[] { tailRight, shaftRight, headRight, tip, headLeft, shaftLeft, tailLeft };

        if (filled)
        {
            Color fillColor = new(color.r, color.g, color.b, color.a * 0.35f);
            Handles.color = fillColor;

            Handles.matrix = local ? transform.localToWorldMatrix : Matrix4x4.identity;

            Handles.DrawAAConvexPolygon(points);
        }

        Handles.color = color;

        Handles.matrix = local ? transform.localToWorldMatrix : Matrix4x4.identity;
            
        float lineWidth = thickness;

        Handles.DrawAAPolyLine(lineWidth, points);
    }

    public static void DrawConicArrow(Transform transform, Vector3 start, Vector3 direction, float size, Color color, float thickness = 1f)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Vector3 tip = start + (direction.normalized * size);
        Handles.color = color;
        Handles.matrix = transform.localToWorldMatrix;

        Handles.DrawLine(start, tip, thickness);
        Handles.ConeHandleCap(0, tip, Quaternion.LookRotation(direction), size * 0.3f, EventType.Repaint);
    }

    public static void DrawNormalCircle(Transform transform, Vector3 center, Vector3 normalDirection, float size, Color color, float thickness = 1f, bool local = true)
    {
        if (normalDirection.sqrMagnitude < 0.0001f)
            return;

        normalDirection.Normalize();

        float circleSize = size * 0.35f;

        Handles.color = color;
        
        Handles.matrix = local ? transform.localToWorldMatrix : Matrix4x4.identity;

        Handles.DrawWireDisc(center, normalDirection, circleSize, thickness);
        
        Handles.color = new(color.r, color.g, color.b, 0.25f);
        Handles.DrawSolidDisc(center, normalDirection, circleSize);
    }

    public static void DrawRaycast(Transform transform, Vector3 origin, Vector3 direction, float length, LayerMask layerMask, Color color, float thickness = 1f)
    {
        direction = direction.sqrMagnitude < 0.0001f ? Vector3.forward : direction.normalized;
        length = Mathf.Max(0.01f, length);

        bool didHit = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            length,
            layerMask,
            QueryTriggerInteraction.UseGlobal
        );

        Vector3 rayEnd = origin + (direction * length);
        Vector3 hitPoint = didHit ? hit.point : rayEnd;
        float sphereSize = 0.1f;

        Handles.matrix = Matrix4x4.identity;
        Handles.color = color;
        Handles.SphereHandleCap(0, origin, Quaternion.identity, sphereSize, EventType.Repaint);
        Handles.SphereHandleCap(0, rayEnd, Quaternion.identity, sphereSize, EventType.Repaint);
        Handles.DrawLine(origin, hitPoint, thickness);

        if (!didHit)
            return;

        Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, sphereSize, EventType.Repaint);
        Handles.DrawDottedLine(hitPoint, rayEnd, 4f);

        DrawNormalArrow(transform, hitPoint, hit.normal.normalized, 1f, color, thickness: thickness, local: false);
        DrawNormalCircle(transform, hitPoint, hit.normal.normalized, 1f, color, thickness: thickness, local: false);
        Handles.matrix = Matrix4x4.identity;
    }

    public static Vector3 DrawRaycastPath(Transform transform, Vector3[] points, LayerMask layerMask, Color color, float thickness = 1f)
    {
        if (points == null || points.Length < 2)
            return Vector3.zero;

        Handles.matrix = Matrix4x4.identity;
        Handles.color = color;

        Vector3 lastPoint = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 currentPoint = points[i];
            Vector3 segment = currentPoint - lastPoint;
            float segmentLength = segment.magnitude;

            if (segmentLength <= 0.0001f)
            {
                lastPoint = currentPoint;
                continue;
            }

            Vector3 direction = segment / segmentLength;

            if (Physics.Raycast(lastPoint, direction, out RaycastHit hit, segmentLength, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                Handles.DrawLine(lastPoint, hit.point, thickness);
                DrawRaycastHit(transform, hit.point, hit.normal, color, thickness);
                return hit.point;
            }

            Handles.DrawLine(lastPoint, currentPoint, thickness);
            lastPoint = currentPoint;
        }

        return points[points.Length - 1];
    }

    private static void DrawRaycastHit(Transform transform, Vector3 hitPoint, Vector3 hitNormal, Color color, float thickness)
    {
        float sphereSize = 0.1f;

        Handles.matrix = Matrix4x4.identity;
        Handles.color = color;
        Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, sphereSize, EventType.Repaint);

        DrawNormalArrow(transform, hitPoint, hitNormal.normalized, 1f, color, thickness: thickness, local: false);
        DrawNormalCircle(transform, hitPoint, hitNormal.normalized, 1f, color, thickness: thickness, local: false);
        Handles.matrix = Matrix4x4.identity;
    }

    public static void DrawWireCube(Transform transform, Vector3 center, Vector3 size, Color color, float thickness = 1f)
    {
        Vector3 halfSize = size * 0.5f;
        Handles.matrix = transform.localToWorldMatrix;

        Vector3 p0 = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        Vector3 p1 = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        Vector3 p2 = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        Vector3 p3 = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        Vector3 p4 = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        Vector3 p5 = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        Vector3 p6 = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
        Vector3 p7 = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

        Handles.color = color;
        Handles.DrawLine(p0, p1, thickness);
        Handles.DrawLine(p1, p2, thickness);
        Handles.DrawLine(p2, p3, thickness);
        Handles.DrawLine(p3, p0, thickness);

        Handles.DrawLine(p4, p5, thickness);
        Handles.DrawLine(p5, p6, thickness);
        Handles.DrawLine(p6, p7, thickness);
        Handles.DrawLine(p7, p4, thickness);

        Handles.DrawLine(p0, p4, thickness);
        Handles.DrawLine(p1, p5, thickness);
        Handles.DrawLine(p2, p6, thickness);
        Handles.DrawLine(p3, p7, thickness);
    }

    public static void DrawWireSphere(Transform transform, Vector3 center, float radius, Color color, float thickness = 1f)
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.color = color;

        Handles.DrawWireDisc(center, Vector3.right, radius, thickness);
        Handles.DrawWireDisc(center, Vector3.up, radius, thickness);
        Handles.DrawWireDisc(center, Vector3.forward, radius, thickness);
    }
#endif
}

public enum AlignmentVertical
{
    Top,
    Center,
    Bottom
}

public enum AlignmentHorizontal
{
    Left,
    Center,
    Right
}

public enum AlignmentDepth
{
    Back,
    Center,
    Front
}
}
