using System;
using UnityEngine;
using System.Reflection;

namespace DuzeraTools.GizmosDrawer
{
[Serializable]
public class FloatReference
{
    public UnityEngine.Object _target;
    public string _componentTypeName;
    public string _variableName;

    public float GetValue()
    {
        if (_target == null || string.IsNullOrEmpty(_componentTypeName) || string.IsNullOrEmpty(_variableName))
            return 0f;

        var type = Type.GetType(_componentTypeName);
        if (type == null) return 0f;

        var targetInstance = ResolveTargetInstance(type);
        if (targetInstance == null) return 0f;

        var field = type.GetField(_variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (field != null && field.FieldType == typeof(float))
            return (float)field.GetValue(targetInstance);

        var prop = type.GetProperty(_variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (prop != null && prop.PropertyType == typeof(float))
            return (float)prop.GetValue(targetInstance);

        return 0f;
    }

    private object ResolveTargetInstance(Type type)
    {
        if (_target is GameObject go)
            return go.GetComponent(type);

        if (_target is Component component)
        {
            if (type.IsInstanceOfType(component))
                return component;

            return component.GetComponent(type);
        }

        if (type.IsInstanceOfType(_target))
            return _target;

        return null;
    }
}
}
