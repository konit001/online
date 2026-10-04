using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace DuzeraTools.GizmosDrawer
{
[CustomEditor(typeof(Gizmo), true)]
public class GizmoEditor : Editor
{
    private static readonly HashSet<string> BasePropertyNames = new()
    {
        "m_Script",
        "_showColor",
        "_showSize",
        "_showReferencedSize",
        "_showThickness",
        "_draw",
        "_selectionOnly",
        "_color",
        "_size",
        "_referencedSize",
        "_thickness"
    };
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSummary();
        EditorGUILayout.Space();

        DrawBaseGizmoSettings();
        EditorGUILayout.Space();
        DrawDerivedSettings();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSummary()
    {
        var summary = target.GetType().GetCustomAttribute<GizmoSummaryAttribute>();
        if (summary == null || string.IsNullOrWhiteSpace(summary.Text))
            return;

        EditorGUILayout.HelpBox(summary.Text, MessageType.Info);
    }

    private void DrawBaseGizmoSettings()
    {
        var gizmo = (Gizmo)target;

        EditorGUILayout.LabelField("Gizmo Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_draw"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_selectionOnly"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Gizmo Settings", EditorStyles.boldLabel);

        if (gizmo.ShowColorField)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_color"));

        if (gizmo.ShowSizeField)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_size"));

        if (gizmo.ShowReferencedSizeField)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_referencedSize"));

        if (gizmo.ShowThicknessField)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_thickness"));
    }

    private void DrawDerivedSettings()
    {
        var iterator = serializedObject.GetIterator();
        var enterChildren = true;
        var drewHeader = false;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (BasePropertyNames.Contains(iterator.propertyPath))
                continue;

            if (!drewHeader)
                drewHeader = true;

            EditorGUILayout.PropertyField(iterator, true);
        }
    }
}
}
#endif
