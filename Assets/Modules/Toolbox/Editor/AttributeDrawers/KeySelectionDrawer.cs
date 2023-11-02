using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CustomPropertyDrawer(typeof(KeySelectionAttribute))]
public class KeySelectionDrawer : PropertyDrawer
{
    #region Main
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorAddons.SingleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        KeySelectionSearchProvider provider = CreateProvider(property);
        DrawLabel(position, property);
        if (DrawButton(position, property))
        {
            SearchWindowContext con = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            SearchWindow.Open(con, provider);
        }

        UnityEngine.Object.DestroyImmediate(provider, true);
    }

    private KeySelectionSearchProvider CreateProvider(SerializedProperty property)
    {
        void UpdateProperty(string value)
        {
            property.stringValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }

        Type type = (attribute as KeySelectionAttribute).ProviderType;
        KeySelectionSearchProvider provider = (KeySelectionSearchProvider)ScriptableObject.CreateInstance(type);
        provider.OnSelected = UpdateProperty;

        if (string.IsNullOrEmpty(property.stringValue))
            UpdateProperty(provider.Keys[0]);

        return provider;
    }

    private void DrawLabel(Rect position, SerializedProperty property)
    {
        string label = property.IsArray()
            ? property.stringValue
            : property.displayName;

        Rect labelRect = new Rect(position);
        labelRect.width = LabelWidht;
        EditorGUI.LabelField(labelRect, label);
    }

    private bool DrawButton(Rect position, SerializedProperty property)
    {
        Rect buttonRect = new Rect(position);
        buttonRect.width = position.width - LabelWidht;
        buttonRect.x = position.x + LabelWidht;
        return GUI.Button(buttonRect, property.stringValue, EditorStyles.popup);
    }

    private float LabelWidht => EditorGUIUtility.labelWidth;
    #endregion
}