using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortLayer))]
public class SortLayerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorAddons.SingleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty nameProp = property.FindPropertyRelative("_name");
        SerializedProperty idProp = property.FindPropertyRelative("_id");

        string[] names = SortingLayer.layers
            .Select(l => l.name)
            .ToArray();
        int selection = names.IndexOf(nameProp.stringValue);
        selection = Mathf.Clamp(selection, 0, names.Length-1);

        GUIContent[] options = names.Select(n => new GUIContent(n)).ToArray();
        selection = EditorGUI.Popup(position, label, selection, options);

        string result = names[selection];
        nameProp.stringValue = result;
        idProp.intValue = SortingLayer.NameToID(result);
        property.serializedObject.ApplyModifiedProperties();
    }
}