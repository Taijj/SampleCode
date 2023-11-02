using System.Collections.Generic;
using UnityEngine;

public static class ObjectAddons
{
	#region GameObjects
	public static void Activate(this GameObject @this) => @this.SetActive(true);
	public static void Activate(this Component @this) => @this.gameObject.Activate();

	public static void Deactivate(this GameObject @this) => @this.SetActive(false);
	public static void Deactivate(this Component @this) => @this.gameObject.Deactivate();
	#endregion



	#region Null Checks
	// Unity overrides the null equality check, which is slower than normal null checks.
	// Use these methods to micro optimize your checks for null and to make clear which
	// null check is used (Unity's or the default one)!

	public static bool IsNull(this Object @this, bool useUnityCheck = false)
    {
        if (useUnityCheck)
            return @this == null;
        return ReferenceEquals(@this, null);
    }

    public static bool HasReference(this Object @this, bool useUnityCheck = false)
    {
        return false == @this.IsNull(useUnityCheck);
    }
    #endregion



    #region Misc
    // Compares objects of any type, repecting all edge cases.
    // See: https://stackoverflow.com/questions/65351/null-or-default-comparison-of-generic-argument-in-c-sharp
    public static bool Equals<T>(T first, T second)
    {
        return EqualityComparer<T>.Default.Equals(first, second);
    }

    public static bool TryCast<T>(this object @this, out T castedObject)
    {
        if (@this is T)
        {
            castedObject = (T)@this;
            return true;
        }

        castedObject = default;
        return false;
    }

    public static bool TryFind<T>(out T result) where T : Object
    {
        result = Object.FindObjectOfType<T>();
        return result.HasReference();
    }
    #endregion
}