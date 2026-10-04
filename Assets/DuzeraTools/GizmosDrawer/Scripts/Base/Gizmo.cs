using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GizmoSummaryAttribute : System.Attribute
{
    public string Text { get; }

    public GizmoSummaryAttribute(string text)
    {
        Text = text;
    }
}

public abstract class Gizmo : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] protected bool _draw = true;
    [SerializeField] protected bool _selectionOnly = false;

    [SerializeField] protected Color _color = Color.white;
    [SerializeField, Min(0.01f)] protected float _size = 1;
    [SerializeField] protected FloatReference _referencedSize;
    [SerializeField, Range(0.01f, 20f)] protected float _thickness = 2;

    protected float Size => HasReference ? _referencedSize.GetValue() : _size;
    private bool HasReference => _referencedSize != null && _referencedSize._target != null;

    private void OnDrawGizmos()
    {
        if (!_draw) return;
        if (_selectionOnly) return;

        Gizmos.color = _color;
        DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_draw) return;
        if (!_selectionOnly) return;

        Gizmos.color = _color;
        DrawGizmo();
    }

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void DrawGizmo() { }

    public virtual void SetTargets(Transform[] transforms) { }

    public void SetDraw(bool draw) => _draw = draw;
    public void SetSelectionOnly(bool selectionOnly) => _selectionOnly = selectionOnly;

    public void SetColor(Color color) => _color = color;
    public void SetSize(float size) => _size = size;
    public void SetReferencedSize(FloatReference referencedSize) => _referencedSize = referencedSize;
    public void SetThickness(float thickness) => _thickness = thickness;

    private bool _showColor = true;
    private bool _showSize = true;
    private bool _showReferencedSize = true;
    private bool _showThickness = true;

    public bool ShowColorField
    {
        get => _showColor;
        protected set => _showColor = value;
    }
    public bool ShowSizeField
    {
        get => _showSize;
        protected set => _showSize = value;
    }
    public bool ShowReferencedSizeField
    {
        get => _showReferencedSize;
        protected set => _showReferencedSize = value;
    }
    public bool ShowThicknessField
    {
        get => _showThickness;
        protected set => _showThickness = value;
    }
#endif
}
}
