using UnityEditor;
using UnityEngine;

namespace Taijj.HeartWarming
{
    [CustomPropertyDrawer(typeof(Heater))]
    public class HeaterDrawer : PropertyDrawer
    {
    	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    	{
    		return EditorGUI.GetPropertyHeight(property, label, true);
    	}

    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
			SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
			Rect rect = new Rect(position);
			rect.width = 15;
			rect.height = EditorAddons.SingleLineHeight;
			rect.x = position.x + EditorGUIUtility.labelWidth;
			enabledProp.boolValue = EditorGUI.Toggle(rect, enabledProp.boolValue);

			GUI.enabled = enabledProp.boolValue;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;

    		property.serializedObject.ApplyModifiedProperties();
    	}
    }
}