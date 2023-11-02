using UnityEngine;

/// <summary>
/// Draws a button that calls a simple method in the inspector. This needs a target
/// property to work. The button will be drawn instead of the property, though.
///
/// Taken from: https://forum.unity.com/threads/attribute-to-add-button-to-class.660262/
/// </summary>
public class ButtonAttribute : PropertyAttribute
{
    public ButtonAttribute(string methodName) => MethodName = methodName;
    public string MethodName { get; }
}