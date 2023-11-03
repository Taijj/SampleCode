using UnityEditor;
using UnityEngine;

namespace Taijj.SampleCode
{
    [CustomPropertyDrawer(typeof(SpanAttribute))]
    public class SpanDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool isValid = fieldInfo.FieldType == typeof(Vector2)
				|| fieldInfo.FieldType == typeof(Vector2Int);

			if(false == isValid)
			{
				EditorGUI.HelpBox(position, "Invalid Property Type! Property must be of Type Vector2 or Vector2Int!",
					MessageType.Error);
				return;
			}

			SpanAttribute span = attribute as SpanAttribute;
			float originalLabelWidth = EditorGUIUtility.labelWidth;

			Rect labelRect = new Rect(position);
			labelRect.width = originalLabelWidth;
			EditorGUI.LabelField(labelRect, label);

			float spacing = position.width * 0.025f;
			float valueTotalWidth = (position.width - labelRect.width) / 2f - spacing;
			float valueLabelWidth = valueTotalWidth * 0.3f;

			EditorGUIUtility.labelWidth = valueLabelWidth;

			SerializedProperty minProperty = property.FindPropertyRelative("x");
			Rect minRect = new Rect(position);
			minRect.x = position.x + labelRect.width;
			minRect.width = valueTotalWidth;
			EditorGUI.PropertyField(minRect, minProperty, new GUIContent(span.MinName), true);

			SerializedProperty maxProperty = property.FindPropertyRelative("y");
			Rect maxRect = new Rect(position);
			maxRect.x = position.x + labelRect.width + minRect.width + spacing;
			maxRect.width = valueTotalWidth;
			EditorGUI.PropertyField(maxRect, maxProperty, new GUIContent(span.MaxName), true);

			EditorGUIUtility.labelWidth = originalLabelWidth;
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}