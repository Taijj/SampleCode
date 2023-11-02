using UnityEngine;
using System;

/// <summary>
/// Draws the field/property ONLY if a comparison expression is true.
/// Not all types are currently supported, since Unity's Serialization
/// API is rather limited. If a type is missing, you need to implement
/// the missing <see cref="Condition">Condition</see> in <see cref="DrawIfDrawer">DrawIfDrawer</see>!
///
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute
{
    public string TargetPropertyName { get; private set; }
    public object CompareValue { get; private set; }
    public bool IsLabelShown { get; private set; }
    public DisableKind Kind { get; private set; }

    public enum DisableKind
    {
        GrayedOut = 0,
        Invisible
    }



    /// <summary>
    /// Only draws the field, if a condition is met.
    /// </summary>
    /// <param name="targetPropertyName">The case sensitive name of the property that is being compared.</param>
    /// <param name="compareValue">The value the target property is being compared to.</param>
    /// <param name="kind">The type of disabling that should happen, if the condition is NOT met.</param>
    /// <param name="isLabelShown">Controls, if the property's label is drawn as well.</param>
    public DrawIfAttribute(string targetPropertyName,
        object compareValue = default,
        bool isLabelShown = true,
        DisableKind kind = DisableKind.Invisible)
    {
        TargetPropertyName = targetPropertyName;
        CompareValue = compareValue;
        IsLabelShown = isLabelShown;
        Kind = kind;
    }
}