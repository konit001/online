using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuzeraTools.GizmosDrawer
{
[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var objProp = property.FindPropertyRelative("_target");
        var compProp = property.FindPropertyRelative("_componentTypeName");
        var memberProp = property.FindPropertyRelative("_variableName");

        Rect labelRect = new(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);

        float contentX = position.x + EditorGUIUtility.labelWidth;
        float contentW = position.width - EditorGUIUtility.labelWidth;
        float objW = contentW * 0.4f;
        float funcW = contentW - objW - 2f;

        Rect objRect = new(contentX, position.y, objW, position.height);
        Rect funcRect = new(contentX + objW + 2f, position.y, funcW, position.height);

        EditorGUI.BeginChangeCheck();
        var newObj = EditorGUI.ObjectField(objRect, GUIContent.none, objProp.objectReferenceValue, typeof(UnityEngine.Object), true);
        if (EditorGUI.EndChangeCheck())
        {
            objProp.objectReferenceValue = newObj;
            compProp.stringValue = "";
            memberProp.stringValue = "";
        }

        var targetObject = objProp.objectReferenceValue;

        string funcLabel = "No Function";
        if (targetObject != null && !string.IsNullOrEmpty(compProp.stringValue) && !string.IsNullOrEmpty(memberProp.stringValue))
        {
            var resolvedType = Type.GetType(compProp.stringValue);
            if (resolvedType != null)
                funcLabel = resolvedType.Name + "/" + memberProp.stringValue;
        }

        if (GUI.Button(funcRect, funcLabel, EditorStyles.popup))
        {
            var serializedObject = property.serializedObject;
            string propPath = property.propertyPath;

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("No Function"), string.IsNullOrEmpty(memberProp.stringValue), () =>
            {
                var p = serializedObject.FindProperty(propPath);
                p.FindPropertyRelative("_componentTypeName").stringValue = "";
                p.FindPropertyRelative("_variableName").stringValue = "";
                serializedObject.ApplyModifiedProperties();
            });

            if (targetObject != null)
            {
                menu.AddSeparator("");

                if (targetObject is GameObject go)
                {
                    foreach (var comp in go.GetComponents<Component>())
                    {
                        Type compType = comp.GetType();
                        string compAQN = compType.AssemblyQualifiedName;
                        string compDisplay = compType.Name;

                        foreach (string member in GetFloatMembers(compType))
                        {
                            string capturedAQN = compAQN;
                            string capturedMember = member;
                            bool isOn = compProp.stringValue == capturedAQN && memberProp.stringValue == capturedMember;

                            menu.AddItem(new GUIContent(compDisplay + "/" + member), isOn, () =>
                            {
                                var p = serializedObject.FindProperty(propPath);
                                p.FindPropertyRelative("_componentTypeName").stringValue = capturedAQN;
                                p.FindPropertyRelative("_variableName").stringValue = capturedMember;
                                serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }
                }
                else
                {
                    Type targetType = targetObject.GetType();
                    string targetAQN = targetType.AssemblyQualifiedName;

                    foreach (string member in GetFloatMembers(targetType))
                    {
                        string capturedAQN = targetAQN;
                        string capturedMember = member;
                        bool isOn = compProp.stringValue == capturedAQN && memberProp.stringValue == capturedMember;

                        menu.AddItem(new GUIContent(targetType.Name + "/" + member), isOn, () =>
                        {
                            var p = serializedObject.FindProperty(propPath);
                            p.FindPropertyRelative("_componentTypeName").stringValue = capturedAQN;
                            p.FindPropertyRelative("_variableName").stringValue = capturedMember;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
            }

            menu.DropDown(funcRect);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    private string[] GetFloatMembers(Type type)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var result = new List<string>();

        foreach (var f in type.GetFields(flags))
            if (f.FieldType == typeof(float) && (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null))
                result.Add(f.Name);

        foreach (var p in type.GetProperties(flags))
            if (p.PropertyType == typeof(float) && p.CanRead)
                result.Add(p.Name);

        return result.ToArray();
    }
}
}
