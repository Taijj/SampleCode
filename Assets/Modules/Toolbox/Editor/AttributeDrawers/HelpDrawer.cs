using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HelpAttribute))]
public class HelpDrawer : PropertyDrawer
{
    #region Lifecycle
    public HelpDrawer() : base()
    {
        Style = new GUIStyle(EditorStyles.helpBox);
        Style.wordWrap = true;
        Style.richText = true;
        Style.fontSize = 13;
    }

    private GUIStyle Style { get; set; }
    private HelpAttribute Attribute => (HelpAttribute)attribute;
    #endregion



    #region Drawing
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        GUIContent content = new GUIContent(Attribute.Message);
        float height = Style.CalcHeight(content, TextWidht);
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIContent iconContent = GetIconContent();
		float iconWidht = 0f;
		if(iconContent.image.HasReference())
			iconWidht = Mathf.Min(position.width, iconContent.image.width);

        Rect iconRect = new Rect(position.x,
            position.y,
            iconContent != GUIContent.none ? iconWidht : 0f,
            position.height);
        Rect textRext = new Rect(position.x + iconRect.width,
            position.y,
            position.width - iconRect.width,
            position.height);

        TextWidht = textRext.width;
        EditorGUI.LabelField(iconRect, GetIconContent());
        EditorGUI.TextArea(textRext, Attribute.Message, Style);
    }

    private float TextWidht { get; set; }
    #endregion



    #region Icon
    private const string INFO_ICON = "console.infoicon";
    private const string WARNING_ICON = "console.warnicon";
    private const string ERROR_ICON = "console.erroricon";

    private GUIContent GetIconContent()
    {
        switch (Attribute.Kind)
        {
            case HelpAttribute.HelpKind.Info: return EditorGUIUtility.IconContent(INFO_ICON);
            case HelpAttribute.HelpKind.Warning: return EditorGUIUtility.IconContent(WARNING_ICON);
            case HelpAttribute.HelpKind.Error: return EditorGUIUtility.IconContent(ERROR_ICON);
            default: return GUIContent.none;
        }
    }
    #endregion
}