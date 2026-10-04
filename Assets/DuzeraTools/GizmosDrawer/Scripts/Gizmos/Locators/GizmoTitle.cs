using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[GizmoSummary("Renders a scene-view icon and label above the object.")]
public class GizmoTitle : Gizmo
{
#if UNITY_EDITOR
    // Draws a scene-view icon and text title at the object position.
    [Header("Title Settings")]
    // Optional sprite icon rendered in screen space.
    [SerializeField] private Sprite _icon;
    // Manual title text used when game object name is not selected.
    [SerializeField] private string _label = "";
    // Uses gameObject.name instead of the custom label.
    [SerializeField] private bool _useGameObjectName = true;

    // Base icon scale factor used for GUI pixel sizing.
    private readonly float _iconSize = 2;
    // Base font size before scaling with gizmo size.
    private readonly int _labelSize = 12;
    
    protected override void DrawGizmo()
    {
        if (ShowThicknessField)
            ShowThicknessField = false;

        DrawIcon();
        DrawLabel();
    }

    private void DrawIcon()
    {
        if (!TryGetIconData(out Texture texture, out Rect uv))
            return;

        Vector2 guiPosition = HandleUtility.WorldToGUIPoint(transform.position);
        float pixelSize = Mathf.Max(8f, _iconSize * 16f * Size);
        Rect iconRect = new(
            guiPosition.x - (pixelSize * 0.5f),
            guiPosition.y - (pixelSize * 0.5f),
            pixelSize,
            pixelSize
        );

        Handles.BeginGUI();
        GUI.DrawTextureWithTexCoords(iconRect, texture, uv, true);
        Handles.EndGUI();
    }

    private void DrawLabel()
    {
        GUIStyle guiStyle = new(EditorStyles.label)
        {
            normal = new GUIStyleState { textColor = _color },
            fontSize = Mathf.RoundToInt(_labelSize * Size),
            alignment = TextAnchor.UpperCenter
        };

        string labelToUse = _useGameObjectName ? gameObject.name : _label;

        Vector2 guiPosition = HandleUtility.WorldToGUIPoint(transform.position);
        float pixelSize = Mathf.Max(8f, _iconSize * 16f);
        Vector2 textSize = guiStyle.CalcSize(new GUIContent(labelToUse));
        Rect labelRect = new(
            guiPosition.x - (textSize.x * 0.5f),
            guiPosition.y + (pixelSize * 0.5f) + 2f,
            textSize.x,
            textSize.y
        );

        Handles.BeginGUI();
        GUI.Label(labelRect, labelToUse, guiStyle);
        Handles.EndGUI();
    }

    private bool TryGetIconData(out Texture texture, out Rect uv)
    {
        if (_icon != null)
        {
            texture = _icon.texture;
            Rect textureRect = _icon.textureRect;
            uv = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height
            );
            return true;
        }

        texture = null;
        uv = default;
        return false;
    }
#endif
}
}
