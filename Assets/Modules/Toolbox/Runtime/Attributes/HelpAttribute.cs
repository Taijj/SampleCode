using UnityEngine;

/// <summary>
/// Shows an information text box in the Inspector, with a different icon depending on its kind.
/// </summary>
public class HelpAttribute : PropertyAttribute
{
    public enum HelpKind
    {
        None = 0,
        Info,
        Warning,
        Error
    }

    public string Message { get; }
    public HelpKind Kind { get; }

    public HelpAttribute(string message, HelpKind kind = HelpKind.None)
    {
        Message = message;
        Kind = kind;
    }
}