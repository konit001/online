using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuzeraTools.GizmosDrawer
{
[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.Vector2)
		{
			EditorGUI.PropertyField(position, property, label, true);
			return;
		}

		MinMaxSliderAttribute slider = (MinMaxSliderAttribute)attribute;

		Rect rowRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

		Vector2 values = property.vector2Value;
		float min = values.x;
		float max = values.y;

		EditorGUI.BeginProperty(position, label, property);
		Rect contentRect = EditorGUI.PrefixLabel(rowRect, label);
		float fieldWidth = 52f;
		float spacing = 4f;

		Rect minRect = new(contentRect.x, contentRect.y, fieldWidth, contentRect.height);
		Rect maxRect = new(contentRect.xMax - fieldWidth, contentRect.y, fieldWidth, contentRect.height);
		Rect sliderRect = new(
			minRect.xMax + spacing,
			contentRect.y,
			Mathf.Max(0f, contentRect.width - (fieldWidth * 2f + spacing * 2f)),
			contentRect.height);

		int previousIndent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		min = EditorGUI.FloatField(minRect, GUIContent.none, min);
		EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, slider.Min, slider.Max);
		max = EditorGUI.FloatField(maxRect, GUIContent.none, max);

		EditorGUI.indentLevel = previousIndent;

		min = Mathf.Clamp(min, slider.Min, slider.Max);
		max = Mathf.Clamp(max, slider.Min, slider.Max);

		if (min > max)
		{
			float temp = min;
			min = max;
			max = temp;
		}

		property.vector2Value = new Vector2(min, max);
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.Vector2)
			return EditorGUI.GetPropertyHeight(property, label, true);

		return EditorGUIUtility.singleLineHeight;
	}
}
}
