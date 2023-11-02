using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        SerializedProperty sceneAsset = property.FindPropertyRelative("_sceneAsset");
        SerializedProperty sceneName = property.FindPropertyRelative("_sceneName");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
        if (sceneAsset.objectReferenceValue != null)
            sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
        else
            sceneName.stringValue = string.Empty;

        EditorGUI.EndProperty();
    }
}