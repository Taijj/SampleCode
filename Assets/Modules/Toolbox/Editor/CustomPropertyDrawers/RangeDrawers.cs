using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntRange))]
public class IntRangeDrawer : RangeDrawer
{
    protected override void Sanitize(SerializedProperty minProperty, SerializedProperty maxProperty)
    {
        if (minProperty.intValue > maxProperty.intValue)
            maxProperty.intValue = minProperty.intValue;
    }
}

[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : RangeDrawer
{
    protected override void Sanitize(SerializedProperty minProperty, SerializedProperty maxProperty)
    {
        if (minProperty.floatValue > maxProperty.floatValue)
            maxProperty.floatValue = minProperty.floatValue;
    }
}

public abstract class RangeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorAddons.SingleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float originalLabelWidth = EditorGUIUtility.labelWidth;

        Rect labelRect = new Rect(position);
        labelRect.width = originalLabelWidth;
        EditorGUI.LabelField(labelRect, label);

        float spacing = position.width * 0.025f;
        float valueTotalWidth = (position.width - labelRect.width) / 2f - spacing;
        float valueLabelWidth = valueTotalWidth * 0.3f;

        EditorGUIUtility.labelWidth = valueLabelWidth;

        SerializedProperty minProperty = property.FindPropertyRelative("_min");
        Rect minRect = new Rect(position);
        minRect.x = position.x + labelRect.width;
        minRect.width = valueTotalWidth;
        EditorGUI.PropertyField(minRect, minProperty, true);

        SerializedProperty maxProperty = property.FindPropertyRelative("_max");
        Rect maxRect = new Rect(position);
        maxRect.x = position.x + labelRect.width + minRect.width + spacing;
        maxRect.width = valueTotalWidth;
        EditorGUI.PropertyField(maxRect, maxProperty, true);

        EditorGUIUtility.labelWidth = originalLabelWidth;
        Sanitize(minProperty, maxProperty);
        property.serializedObject.ApplyModifiedProperties();
    }

    protected abstract void Sanitize(SerializedProperty minProperty, SerializedProperty maxProperty);
}