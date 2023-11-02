using System;
using System.Collections.Generic;

public static class ArrayAddons
{
	/// <summary>
	/// Simply returns the last element in the array.
	/// </summary>
	public static T Last<T>(this T[] @this) => @this[@this.Length-1];

	/// <summary>
	/// Simply returns the last element in the list.
	/// </summary>
	public static T Last<T>(this List<T> @this) => @this[@this.Count-1];



    /// <summary>
    /// Returns the first index of the given item, -1 if the item cannot be found.
    /// </summary>
    public static int IndexOf<T>(this T[] @this, T item)
    {
        for(int i = 0; i < @this.Length; i++)
        {
            if (ObjectAddons.Equals(@this[i], item))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns true, if this array contains the given item, false otherwise.
    /// </summary>
    public static bool Contains<T>(this T[] @this, T item)
    {
        for (int i = 0; i < @this.Length; i++)
        {
            if (ObjectAddons.Equals(@this[i], item))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to add the given item to the array. It will look
    /// for the first "default" item and puts the given item there.
    /// Throws an error, if none can be found.
    /// </summary>
    public static void Add<T>(this T[] @this, T item)
    {
        for(int i = 0; i < @this.Length; i++)
        {
            if (ObjectAddons.Equals(@this[i], default))
            {
                @this[i] = item;
                return;
            }
        }
        throw new Exception("Item couldn't be added. Array does not contain null item!");
    }

    /// <summary>
    /// Replaces the given item in the array with a "default" item.
    /// If the item is not found in the array, nothing happens.
    /// </summary>
    public static void Remove<T>(this T[] @this, T item)
    {
        for(int i = 0; i < @this.Length; i++)
        {
            if (ObjectAddons.Equals(@this[i], item))
            {
                @this[i] = default;
                return;
            }
        }
    }



    /// <summary>
    /// Returns a random item from the array.
    /// </summary>
    public static T Random<T>(this T[] @this)
    {
        try
        {
            return @this[UnityEngine.Random.Range(0, @this.Length)];
        }
        catch(Exception e)
        {
            if (e is IndexOutOfRangeException)
                throw new ArgumentException("Cannot retrieve random element of empty array!");
            else
                throw e;
        }
    }

    /// <summary>
    /// Returns true, if the array is null, empty, or contains any default (i.e. null)' elements, false otherwise.
    /// </summary>
    public static bool IsFaulty<T>(this T[] @this)
    {
        if (@this == null || @this.Length == 0)
            return true;

        foreach (T element in @this)
        {
            if (ObjectAddons.Equals<T>(element, default))
                return true;
        }
        return false;
    }

}