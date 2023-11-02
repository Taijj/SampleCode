
using UnityEngine;

public static class ColorAddons
{
    /// <summary>
    /// Returns the same Color with the given alpha value.
    /// </summary>
    public static Color With(this Color @this, float alpha)
    {
        return new Color(@this.r, @this.g, @this.b, alpha);
    }

    /// <summary>
    /// Returns true, if this Color is similar to the given one, false otherwise.
    /// </summary>
    public static bool IsSimilarTo(this Color @this, Color otherCol)
    {
        return Mathf.Approximately(@this.r, otherCol.r) &&
                Mathf.Approximately(@this.g, otherCol.g) &&
                Mathf.Approximately(@this.b, otherCol.b) &&
                Mathf.Approximately(@this.a, otherCol.a);
    }



    //Additional Colors based on:
    //https://answers.unity.com/questions/785696/global-list-of-colour-names-and-colour-values.html
    public static Color Lime => new Color(0.651f, 0.996f, 0f, 1f);
    public static Color Green => new Color(0f, 0.996f, 0.434f, 1f);
    public static Color Aqua => new Color(0f, 0.785f, 0.996f, 1f);
    public static Color Blue => new Color(0f, 0.477f, 0.996f, 1f);
    public static Color Navy => new Color(0.234f, 0f, 0.996f, 1f);
    public static Color Purple => new Color(0.559f, 0f, 0.996f, 1f);
    public static Color Pink => new Color(0.906f, 0f, 0.996f, 1f);
    public static Color Red => new Color(0.996f, 0.035f, 0f, 1f);
    public static Color Orange => new Color(0.996f, 0.629f, 0f, 1f);
    public static Color Yellow => new Color(0.996f, 0.875f, 0f, 1f);
}