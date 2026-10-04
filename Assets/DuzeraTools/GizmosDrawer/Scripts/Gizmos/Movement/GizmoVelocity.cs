using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Samples movement over time and draws a speed/velocity direction indicator.")]
public class GizmoVelocity : Gizmo
{
#if UNITY_EDITOR
    // Samples transform motion over editor time and draws a movement arrow.
    [Header("Velocity Settings")]
    // Toggles speed magnitude label near the arrow tip.
    [SerializeField] private bool _showSpeedLabel = true;
    // Minimum speed required before drawing to avoid tiny jitter arrows.
    [SerializeField, Min(0.01f)] private float _minimumSpeedToDraw = 0.01f;

    // Previous sample position used to compute frame-to-frame motion.
    private Vector3 _previousPosition;
    // Previous editor timestamp used to compute delta time.
    private double _previousTime;
    // Initialization flag to skip velocity calc on the very first sample.
    private bool _hasPreviousSample;

    protected override void DrawGizmo()
    {
        Vector3 currentPosition = transform.position;
        double currentTime = EditorApplication.timeSinceStartup;

        if (!_hasPreviousSample)
        {
            _previousPosition = currentPosition;
            _previousTime = currentTime;
            _hasPreviousSample = true;
            return;
        }

        double deltaTime = currentTime - _previousTime;
        if (deltaTime <= 0.00001d)
            return;

        Vector3 velocity = (currentPosition - _previousPosition) / (float)deltaTime;
        float speed = velocity.magnitude;

        _previousPosition = currentPosition;
        _previousTime = currentTime;

        if (speed < _minimumSpeedToDraw)
            return;

        Vector3 direction = velocity / speed;
        float arrowLength = Mathf.Max(0.01f, Size);

        Vector3 shaftStart = currentPosition;
        Vector3 tip = shaftStart + direction * arrowLength;

        GizmoUtils.DrawNormalArrow(transform, shaftStart, direction, arrowLength, _color, thickness: _thickness, local: false);

        if (_showSpeedLabel)
            GizmoUtils.DrawLabel(tip + direction * 0.08f, $"{speed:F2} u/s", _color, TextAnchor.LowerCenter);
    }
#endif
}
}
