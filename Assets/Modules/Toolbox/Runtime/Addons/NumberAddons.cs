
using UnityEngine;

public static class NumberAddons
{
    public static int Sign(this int @this, bool canBeZero = false)
    {
        if (canBeZero)
            return System.Math.Sign(@this);
        return (int)Mathf.Sign(@this);
    }

    public static int Sign(this float @this, bool canBeZero = false)
    {
        if(canBeZero)
            return System.Math.Sign(@this);
        return (int)Mathf.Sign(@this);
    }



    /// <summary>
    /// Returns true, if the int is greater or equal to min, and lesser or equal to max.
    /// </summary>
    public static bool IsBetween(this int @this, int min, int max)
    {
        return @this >= min && @this <= max;
    }

    /// <summary>
    /// Returns true, if the float is greater or equal to min, and lesser or equal to max.
    /// </summary>
    public static bool IsBetween(this float @this, float min, float max)
    {
        return @this >= min && @this <= max;
    }

    /// <summary>
    /// Returns true, if the int is near the given value, false otherwise.
    /// </summary>
    /// <param name="tolerance">Defines how much lesser[inclusive] or greater[inclusive] the int can be in order to count as "near".</param>
    public static bool IsAround(this int @this, int value, int tolerance)
    {
        return @this >= value-tolerance && @this <= value+tolerance;
    }

    /// <summary>
    /// Returns true, if the float is near the given value, false otherwise.
    /// </summary>
    /// <param name="tolerance">Defines how much lesser[inclusive] or greater[inclusive] the float can be in order to count as "near".</param>
    public static bool IsAround(this float @this, float value, float tolerance)
    {
        return @this >= value-tolerance && @this <= value+tolerance;
    }



    /// <summary>
    /// Returns true or false at random.
    /// </summary>
    public static bool RandomBool => Random.value > 0.5f;



    /// <summary>
    /// Converts the number to a display string, with leading zeros.
    /// </summary>
    /// <param name="length">The length of the resulting string.</param>
    /// <returns></returns>
    public static string ToStringLeadingZeros(this int @this, int length)
    {
        int digits = @this < 0 ? length - 1 : length;
        string format = string.Format(LEADING_ZERO_FORMAT, digits);
        return @this.ToString(format);
    }

    private const string LEADING_ZERO_FORMAT = "D{0}";



    /// <summary>
    /// Treats the number as an angle and returns it as a direction.
    /// </summary>
    public static Vector2 ConvertToDirection(this float @this)
    {
        float radians = @this * Mathf.Deg2Rad;

        // Round here to prevent very, very small values.
        float cos = (float)System.Math.Round(Mathf.Cos(radians), DIRECTION_ROUND_DIGITS);
        float sin = (float)System.Math.Round(Mathf.Sin(radians), DIRECTION_ROUND_DIGITS);
        return new Vector2(cos, sin);
    }

    private const int DIRECTION_ROUND_DIGITS = 5;
}