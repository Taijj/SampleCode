
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TransformAddons
{
	#region 2D Facing
	/// <summary>
	/// Sets the 2D left/right direction of this Transform.
	/// Will rotate along the y axis to 0 for "facing right" and 180 for "facing left".
	/// </summary>
	/// <param name="direction">Facing will be set depending on this direction's x value's sign.</param>
	/// <returns>-1 for "facing left" 1 for "facing right"</returns>
	public static int SetFacing(this Transform @this, Vector2 direction) => SetFacing(@this, direction.x);

	/// <summary>
	/// Sets the 2D left/right direction of this Transform.
	/// Will rotate along the y axis to 0 for "facing right" and 180 for "facing left".
	/// </summary>
	/// <param name="value">Facing will be set depending value's sign.</param>
	/// <returns>-1 for "facing left" 1 for "facing right"</returns>
	public static int SetFacing(this Transform @this, float value)
	{
		int sign = value.Sign(true);
		if (sign == 0)
			return @this.GetFacing();

		float rotation = sign < 0 ? 180f : 0f;
		@this.localEulerAngles = Vector3.up * rotation;
		return sign;
	}

	/// <summary>
	/// Gets the 2D left/right direction of the given transform, depending
	/// on its y axis roation.
	/// </summary>
	/// <returns>-1 for "facing left" 1 for "facing right"</returns>
	public static int GetFacing(this Transform @this) => @this.localEulerAngles.y == 0f ? 1 : -1;
	#endregion




	/// <summary>
	/// Logs the Transform's hierarchy in a path like syntax to the Unity console.
	/// Note: This method is meant for debugging, and is therefore rather expensive!
	/// </summary>
	public static void LogHierarchy(this Transform @this)
    {
        List<string> steps = new List<string>() { @this.name };

        Transform nextParent = @this.parent;
        while(nextParent.HasReference())
        {
            steps.Add(nextParent.name);
            nextParent = nextParent.parent;
        }
        steps.Reverse();

        StringBuilder builder = new StringBuilder();
        foreach (string step in steps)
            builder.Append($"{step}/");

        Note.Log(builder.ToString());
    }
}