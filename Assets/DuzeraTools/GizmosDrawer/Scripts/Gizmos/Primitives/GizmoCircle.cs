using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Draws a circle or arc around the object based on arc angle.")]
public class GizmoCircle : Gizmo
{
#if UNITY_EDITOR
    // Draws a circular arc around the transform position.
    [Header("Circle Settings")]
    // Arc sweep in degrees (360 = full circle).
    [SerializeField, Range(0f, 360f)] private float _arcAngle = 360f;
    // When enabled, draws a line to close the arc at start and end points.
    [SerializeField] private bool _closedArc = true;

    protected override void DrawGizmo()
    {
        Handles.color = _color;
        Handles.matrix = transform.localToWorldMatrix;

        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, _arcAngle, Size * 0.5f, _thickness);

        if (_closedArc && _arcAngle < 360f)
        {
            Vector3 startDirection = Quaternion.AngleAxis(0f, Vector3.up) * Vector3.forward;
            Vector3 endDirection = Quaternion.AngleAxis(_arcAngle, Vector3.up) * Vector3.forward;
            Handles.DrawLine(Vector3.zero, 0.5f * Size * startDirection, _thickness);
            Handles.DrawLine(Vector3.zero, 0.5f * Size * endDirection, _thickness);
        }
    }
#endif
}
}
