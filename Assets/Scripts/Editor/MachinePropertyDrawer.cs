using UnityEditor;
using UnityEngine;

namespace Taijj.HeartWarming
{
	[CustomPropertyDrawer(typeof(MachineConfig.Property), true)]
	public class MachinePropertyDrawer : PropertyDrawer
    {
    	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    	{
			return EditorAddons.SingleLineHeight;
    	}

    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
			SerializedProperty enabledProp = property.FindPropertyRelative("_enabled");
			SerializedProperty injectionProp = property.FindPropertyRelative("_injection");

			GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);
			toggleStyle.margin = new RectOffset();
			Rect enabledRect = new Rect(position);
			enabledRect.width = TOGGLE_WIDTH;
			enabledProp.boolValue = EditorGUI.Toggle(enabledRect, enabledProp.boolValue, toggleStyle);

			bool guiWasEnabled = GUI.enabled;
			GUI.enabled = enabledProp.boolValue;
			float remainingWidht = position.width - TOGGLE_WIDTH;
			float x = position.x + TOGGLE_WIDTH;

			Rect labelRect = new Rect(position);
			labelRect.x = x;
			labelRect.width = remainingWidht * LABEL_WIDTH;
			EditorGUI.LabelField(labelRect, label);

			Rect injectionRect = new Rect(position);
			injectionRect.x = x + remainingWidht * LABEL_WIDTH;
			injectionRect.width = remainingWidht * (1f - LABEL_WIDTH);
			EditorGUI.PropertyField(injectionRect, injectionProp, GUIContent.none, true);

			GUI.enabled = guiWasEnabled;
			property.serializedObject.ApplyModifiedProperties();
    	}

		private const int TOGGLE_WIDTH = 20;
		private const float LABEL_WIDTH = 0.3f;
    }
}