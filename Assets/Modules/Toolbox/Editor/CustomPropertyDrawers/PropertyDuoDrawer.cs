using UnityEditor;
using UnityEngine;

/// <summary>
/// Data struct for configuring the drawing of a PropertyDuo.
/// </summary>
public struct PropertyDuoData
{
    public SerializedProperty mainProperty;
    public Rect position;

    public string propertyName1;
    public string propertyName2;

    public string label1;
    public string label2;

    // Normalized
    private const float DEFAULT_PROPERTY_WIDTH_1 = 0.5f;
    private const float DEFAULT_PROPERTY_WIDTH_2 = 0.3f;
    private const float DEFAULT_LABEL_WIDTH_2 = 0.2f;

    public float fieldWidth1;
    public float fieldWidth2;
    public float labelWidth2;



    public float PropertyWidth1 => fieldWidth1 == 0 ? DEFAULT_PROPERTY_WIDTH_1 : fieldWidth1;
    public float PropertyWidth2 => fieldWidth2 == 0 ? DEFAULT_PROPERTY_WIDTH_2 : fieldWidth2;
    public float LabelWidth2 => labelWidth2 == 0 ? DEFAULT_LABEL_WIDTH_2 : labelWidth2;
}

/// <summary>
/// Base class to draw PropertyDuos. These are Serializable Properties, that consist of two
/// separated Properties that should be drawn in a single line in the inspector.
///
/// To use this, implement your own custom PropertyDrawer that extends this class. Then use
/// the Draw() method with the needed PropertyDuoData in OnGUI() to configure how your
/// Property is drawn to the inspector.
/// </summary>
public class PropertyDuoDrawer : PropertyDrawer
{
    #region Main
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorAddons.SingleLineHeight;
    }

    public void Draw(PropertyDuoData data)
    {
        WidthWithoutMainLabel = data.position.width - EditorGUIUtility.labelWidth;
        PropertyWidth1 = EditorGUIUtility.labelWidth + WidthWithoutMainLabel*data.PropertyWidth1;

        DrawFirstProperty(data);
        DrawSecondProperty(data);

        data.mainProperty.serializedObject.ApplyModifiedProperties();
    }

    private void DrawFirstProperty(PropertyDuoData data)
    {
        SerializedProperty prop1 = data.mainProperty.FindPropertyRelative(data.propertyName1);
        Rect propRect1 = new Rect(data.position);
        propRect1.width = PropertyWidth1;
        string label1 = string.IsNullOrEmpty(data.label1) ? prop1.displayName : data.label1;
        EditorGUI.PropertyField(propRect1, prop1, new GUIContent(label1));
    }

    private void DrawSecondProperty(PropertyDuoData data)
    {
        SetCustomStyling(WidthWithoutMainLabel*data.LabelWidth2);

        SerializedProperty prop2 = data.mainProperty.FindPropertyRelative(data.propertyName2);
        Rect propRect2 = new Rect(data.position);
        propRect2.x = propRect2.x + PropertyWidth1;
        propRect2.width = EditorGUIUtility.labelWidth + WidthWithoutMainLabel*data.PropertyWidth2;
        string label2 = string.IsNullOrEmpty(data.label2) ? prop2.displayName : data.label2;
        EditorGUI.PropertyField(propRect2, prop2, new GUIContent(label2));

        RestoreOriginalStyling();
    }

    private float WidthWithoutMainLabel { get; set; }
    private float PropertyWidth1 { get; set; }
    #endregion



    #region Custom Styling
    private void SetCustomStyling(float labelWidth)
    {
        OriginalEditorLabelWidth = EditorGUIUtility.labelWidth;
        OriginalEditorLabelAnchor = EditorStyles.label.alignment;
        OriginalEditorLabelPadding = EditorStyles.label.margin;

        EditorGUIUtility.labelWidth = labelWidth;
        EditorStyles.label.alignment = TextAnchor.MiddleRight;
        EditorStyles.label.padding = new RectOffset(10, 10, 0, 0);
    }

    private void RestoreOriginalStyling()
    {
        EditorGUIUtility.labelWidth = OriginalEditorLabelWidth;
        EditorStyles.label.alignment = OriginalEditorLabelAnchor;
        EditorStyles.label.padding = OriginalEditorLabelPadding;
    }

    private float OriginalEditorLabelWidth { get; set; }
    private TextAnchor OriginalEditorLabelAnchor { get; set; }
    private RectOffset OriginalEditorLabelPadding { get; set; }
    #endregion
}