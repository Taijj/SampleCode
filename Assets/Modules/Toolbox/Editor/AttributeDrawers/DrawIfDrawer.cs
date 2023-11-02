using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfDrawer : PropertyDrawer
{
    #region Drawing
    private DrawIfAttribute Attribute { get; set; }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Attribute = attribute as DrawIfAttribute;
        float defaultHeight = EditorGUI.GetPropertyHeight(property, label, true);
        if (Attribute.Kind == DrawIfAttribute.DisableKind.GrayedOut)
            return defaultHeight;

        if (IsConditionMet(property))
            return defaultHeight;

        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Attribute = attribute as DrawIfAttribute;
        if (IsConditionMet(property))
        {
			Draw(position, property, label);
			return;
        }

        if (Attribute.Kind == DrawIfAttribute.DisableKind.GrayedOut)
        {
            GUI.enabled = false;
			Draw(position, property, label);
            GUI.enabled = true;
        }
    }

	private void Draw(Rect position, SerializedProperty property, GUIContent label)
	{
        EditorGUI.PropertyField(position, property, Attribute.IsLabelShown ? label : null, true);
	}
    #endregion



    #region Comparisons
    private bool IsConditionMet(SerializedProperty property)
    {
        string path = property.propertyPath.Contains(".")
            ? System.IO.Path.ChangeExtension(property.propertyPath, Attribute.TargetPropertyName)
            : Attribute.TargetPropertyName;

        SerializedProperty prop = property.serializedObject.FindProperty(path);
        if (prop == null)
        {
            Note.LogError("Cannot find property with name: " + path);
            return true;
        }

        if(false == TryGetCondition(prop.propertyType, out Condition condition))
        {
            Note.LogError($"Serialized type {prop.propertyType} not supported, yet! Condition must be implemented to add support!");
            return true;
        }
        return condition.IsMet(prop, Attribute.CompareValue);
    }

    private bool TryGetCondition(SerializedPropertyType type, out Condition condition)
    {
        condition = null;
        for(int i = 0; i < Conditions.Length; i++)
        {
            if (Conditions[i].Type == type)
            {
                condition = Conditions[i];
                return true;
            }
        }
        return false;
    }
    #endregion



    #region Condition Definitions
    private abstract class Condition
    {
        public bool IsMet(SerializedProperty target, object compareValue)
        {
            if (compareValue == null)
                compareValue = DefaultCompareValue;

            return Compare(target, compareValue);
        }

        protected abstract object DefaultCompareValue { get; }
        protected abstract bool Compare(SerializedProperty target, object compareValue);

        public abstract SerializedPropertyType Type { get; }
    }

    private class BoolCondition : Condition
    {
        protected override object DefaultCompareValue => true;
        protected override bool Compare(SerializedProperty target, object compareValue) => target.boolValue.Equals(compareValue);
        public override SerializedPropertyType Type => SerializedPropertyType.Boolean;
    }

    private class IntCondition : Condition
    {
        protected override object DefaultCompareValue => 0;
        protected override bool Compare(SerializedProperty target, object compareValue) => target.intValue.Equals(compareValue);
        public override SerializedPropertyType Type => SerializedPropertyType.Integer;
    }

    private class FloatCondition : Condition
    {
        protected override object DefaultCompareValue => 0f;
        protected override bool Compare(SerializedProperty target, object compareValue) => target.floatValue.Equals(compareValue);
        public override SerializedPropertyType Type => SerializedPropertyType.Float;
    }

    private class StringCondition : Condition
    {
        protected override object DefaultCompareValue => string.Empty;
        protected override bool Compare(SerializedProperty target, object compareValue) => target.stringValue.Equals(compareValue);
        public override SerializedPropertyType Type => SerializedPropertyType.String;
    }

    private class EnumCondition : Condition
    {
        protected override object DefaultCompareValue => 0;
        protected override bool Compare(SerializedProperty target, object compareValue)
        {
            return target.enumValueFlag.Equals((int)compareValue);
        }
        public override SerializedPropertyType Type => SerializedPropertyType.Enum;
    }

    private class ObjectCondition : Condition
    {
        protected override object DefaultCompareValue => null;
        protected override bool Compare(SerializedProperty target, object compareValue) => target.objectReferenceValue != null;
        public override SerializedPropertyType Type => SerializedPropertyType.ObjectReference;
    }

    // Add more conditions by type here, and add them to the following array!

    private static readonly Condition[] Conditions = new Condition[]
    {
        new BoolCondition(),
        new IntCondition(),
        new FloatCondition(),
        new StringCondition(),
        new EnumCondition(),
        new ObjectCondition()
    };
    #endregion
}