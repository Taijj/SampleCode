using UnityEngine;

/// <summary>
/// Disables interaction for an inspector field and draws it in gray.
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{
    public ReadOnlyAttribute() {}
}