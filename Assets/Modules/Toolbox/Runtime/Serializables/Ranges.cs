using System;
using UnityEngine;

/// <summary>
/// Defines a range from a max to a min value.
/// Values are inclusive!
/// </summary>
[Serializable]
public struct IntRange
{
    [SerializeField] private int _min;
    [SerializeField] private int _max;

    public int Min => _min;
    public int Max => _max;
    public int Delta => Max - Min;
    public int Random => Mathf.RoundToInt(_min + UnityEngine.Random.value * Delta);

    /// <param name="min">[Inclusive]</param>
    /// <param name="max">[Inclusive]</param>
    public IntRange(int min, int max)
    {
        _max = min > max ? min : max;
        _min = min > max ? max : min;
    }

    public int Clamp(int value) => Mathf.Clamp(value, Min, Max);

    /// <summary>
    /// Is the value inside this range? [Inclusive]
    /// </summary>
    public bool Contains(int value) => value >= _min && value <= _max;

    /// <summary>
    /// Loops the given value around to fit it into this Range.
    /// </summary>
    public int Loop(int value)
    {
        if (Contains(value))
            return value;

        int shift = -_min;
        int val0 = value + shift;
        int mod0 = val0 % (Delta+1);
        if (mod0 < 0)
            return _max + mod0 + 1;
        if (mod0 > 0)
            return _min + mod0;
        return _min;
    }

    /// <summary>
    /// Returns a random value within this range that is different from the given value,
    /// and is a given distance away.
    /// </summary>
    /// <param name="lastValue">The old value, which will be updated with this method, too.</param>
    /// <param name="minDelta">The minimum delta the new value should have from the old.</param>
    /// <returns></returns>
    public int RandomDifferentFrom(ref int lastValue, int minDelta = 1)
    {
        if(ClampAndHandleSpecialCases(ref lastValue))
            return lastValue;

        if (minDelta < 1)
            minDelta = 1;

        int result;
        int totalRange = Delta + 1;
        if (minDelta < totalRange/2)
        {
            int remainingDelta = totalRange - 2 * minDelta;

            float random = UnityEngine.Random.value;
            float next = lastValue + minDelta + random * remainingDelta;
            result = Mathf.FloorToInt(next);
        }
        else
        {
            result = lastValue + totalRange/2;
        }

        if (result > _max)
            result -= totalRange;

        lastValue = result;
        return lastValue;
    }

    private bool ClampAndHandleSpecialCases(ref int lastValue)
    {
        if (Delta == 0)
        {
            lastValue = _min;
            return true;
        }
        else if (Delta == 1) // alternate between min and max
        {
            lastValue = lastValue == _min ? _max : _min;
            return true;
        }

        lastValue = Clamp(lastValue);
        if (Delta == 2) // better distributed random between 3 values
        {
            lastValue += NumberAddons.RandomBool ? 1 : 2;
            if (lastValue > _max)
                lastValue -= Delta + 1;
            return true;
        }
        return false;
    }
}




[Serializable]
public struct FloatRange
{
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    public float Min => _min;
    public float Max => _max;
    public float Delta => Max - Min;
    public float Middle => Min + Delta / 2f;
    public float Random => _min + UnityEngine.Random.value * Delta;

    public FloatRange(float min, float max)
    {
        _max = max;
        _min = min;
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, Min, Max);
    }

	/// <summary>
	/// Is the value inside this range? [Inclusive]
	/// </summary>
	public bool Contains(float value) => value >= _min && value <= _max;

	/// <summary>
	/// Loops the given value around to fit it into this Range.
	/// </summary>
	public float Loop(float value)
    {
        if (value < Min)
            return value + Delta;
        if (value > Max)
            return value - Delta;
        return value;
    }

    /// <summary>
    /// Returns a random value within this range that is guaranteed to be a given distance away.
    /// </summary>
    /// <param name="minDistance">The guaranteed minimum distance the new value will have from the old.</param>
    /// <param name="lastValue">The old value, which will be updated with this method, too.</param>
    /// <returns></returns>
    public float RandomWithMinDistance(float minDistance, ref float lastValue)
    {
        float remainder = Delta - 2f * minDistance;
        if (remainder < 0f)
            Note.LogWarning("MinDistance cannot be guaranteed, because Range is too small for minDistance!");

        if (remainder == 0f)
            Note.LogWarning("MinDistance is exactly half. Random will only alternate between two values!");

        lastValue = Clamp(lastValue);
        float random = lastValue + minDistance + remainder * UnityEngine.Random.value;
        float result = Loop(random);

        lastValue = result;
        return result;
    }
}