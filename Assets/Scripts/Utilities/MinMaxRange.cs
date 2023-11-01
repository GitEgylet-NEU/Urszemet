using UnityEditor;
using UnityEngine;

namespace GitEgylet.Utilities
{
	[System.Serializable]
	public class MinMaxRange
	{
		[SerializeField] float min;
		[SerializeField] float max;

		public MinMaxRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float Get()
		{
			return Random.Range(min, max);
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(MinMaxRange))]
	public class MinMaxRangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.serializedObject.Update();
			EditorGUI.BeginProperty(position, label, property);

			// Indent the content by the standard amount
			position = EditorGUI.IndentedRect(position);

			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = EditorGUIUtility.standardVerticalSpacing;

			// Calculate the width for the label and property fields
			float labelWidth = EditorGUIUtility.labelWidth;
			float propertyFieldWidth = (position.width - labelWidth) * 0.5f - spacing;

			// Draw the label
			Rect labelRect = new Rect(position.x, position.y, labelWidth, lineHeight);
			EditorGUI.LabelField(labelRect, label);

			// Adjust the position for the property fields
			position.x += labelWidth + spacing;
			position.width = propertyFieldWidth;

			// Draw the min and max fields
			EditorGUI.PropertyField(position, property.FindPropertyRelative("min"), GUIContent.none, true);

			position.x += propertyFieldWidth + spacing;

			EditorGUI.PropertyField(position, property.FindPropertyRelative("max"), GUIContent.none, true);

			EditorGUI.EndProperty();
			property.serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}