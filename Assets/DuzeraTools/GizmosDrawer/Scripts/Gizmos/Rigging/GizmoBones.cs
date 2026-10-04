using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[ExecuteInEditMode]
[GizmoSummary("Draws the bones of a rigged character or object, showing the hierarchy and connections between joints.")]
public class GizmoBones : Gizmo
{
#if UNITY_EDITOR
    // Draws kite-like connections through the transform hierarchy to represent bones.
    [Header("Bones Settings")]
    // When enabled, draws spheres at each joint position to enhance visibility.
    [SerializeField] bool _drawBoneSpheres = true;
    // Color for the joint spheres, with some transparency to avoid obscuring the bone connections.
    [SerializeField] private Color _spheresColor = new(1f, 1f, 1f, 0.5f);
    // Relative size of the joint spheres based on the bone length, with a minimum threshold to ensure visibility.
    [SerializeField] private float _sphereRadius = 0.2f;

    private readonly float _kiteWidthRatio = 0.125f;
    private readonly float _minKiteWidth = 0.001f;
    private readonly float _kitePivot = 0.2f;
    
    protected override void DrawGizmo()
    {
        DrawBoneHierarchy(transform);
    }

    private void DrawBoneHierarchy(Transform parent)
    {
        if (parent == null)
            return;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Vector3 direction = child.position - parent.position;

            if (direction.sqrMagnitude > 0.000001f)
            {
                Handles.color = _color;
                DrawKiteBone(parent.position, child.position);
            }

            DrawBoneHierarchy(child);
        }
    }

    private void DrawKiteBone(Vector3 start, Vector3 end)
    {
        Vector3 boneDirection = end - start;
        float length = boneDirection.magnitude;
        if (length <= 0.000001f)
            return;

        boneDirection /= length;

        Vector3 viewForward = Camera.current != null ? Camera.current.transform.forward : Vector3.forward;
        Vector3 side = Vector3.Cross(boneDirection, viewForward);

        if (side.sqrMagnitude < 0.000001f)
            side = Vector3.Cross(boneDirection, Vector3.up);
        if (side.sqrMagnitude < 0.000001f)
            side = Vector3.Cross(boneDirection, Vector3.right);

        side.Normalize();
        Vector3 up = Vector3.Cross(boneDirection, side).normalized;

        float halfWidth = Mathf.Max(_minKiteWidth, length * _kiteWidthRatio);
        Vector3 center = Vector3.Lerp(start, end, _kitePivot);

        Vector3 sideA = center + side * halfWidth;
        Vector3 sideB = center - side * halfWidth;
        Vector3 upA = center + up * halfWidth;
        Vector3 upB = center - up * halfWidth;

        Color fillColor = new(_color.r, _color.g, _color.b, _color.a * 0.3f);
        Handles.color = fillColor;
        Handles.DrawAAConvexPolygon(start, sideA, end, sideB);
        Handles.DrawAAConvexPolygon(start, upA, end, upB);

        Handles.color = _color;

        DrawKiteEdges(start, end, sideA, sideB);
        DrawKiteEdges(start, end, upA, upB);

        Handles.DrawLine(sideA, upA, _thickness);
        Handles.DrawLine(upA, sideB, _thickness);
        Handles.DrawLine(sideB, upB, _thickness);
        Handles.DrawLine(upB, sideA, _thickness);

        if (!_drawBoneSpheres)
            return;

        float halfRadius = Mathf.Max(_minKiteWidth, length * _sphereRadius);
        DrawKiteSphere(start, halfRadius);
    }

    private void DrawKiteEdges(Vector3 start, Vector3 end, Vector3 a, Vector3 b)
    {
        Handles.DrawLine(start, a, _thickness);
        Handles.DrawLine(a, end, _thickness);
        Handles.DrawLine(end, b, _thickness);
        Handles.DrawLine(b, start, _thickness);
    }

    private void DrawKiteSphere(Vector3 position, float radius)
    {
        Handles.color = _spheresColor;
        Handles.SphereHandleCap(0, position, Quaternion.identity, radius, EventType.Repaint);
    }
#endif
}
}
