using UnityEngine;

public static class VectorAddons
{
    public static Vector2 WithX(this Vector2 @this, float x) => new Vector2(x, @this.y);
    public static Vector2 WithY(this Vector2 @this, float y) => new Vector2(@this.x, y);

    public static Vector3 WithX(this Vector3 @this, float x) => new Vector3(x, @this.y, @this.z);
    public static Vector3 WithY(this Vector3 @this, float y) => new Vector3(@this.x, y, @this.z);
    public static Vector3 WithZ(this Vector3 @this, float z) => new Vector3(@this.x, @this.y, z);



    /// <summary>
    /// Returns a random point inside this Rect in world space.
    /// </summary>
    public static Vector2 GetRandomPoint(this Rect @this)
    {
        return new Vector2(
            @this.x + @this.width * Random.value,
            @this.y + @this.height * Random.value);
    }



    /// <summary>
    /// Tries to get the point where two lines meet.
    /// From: https://blog.dakwamine.fr/?p=1943
    /// </summary>
    /// <param name="a1">A point on the first line.</param>
    /// <param name="a2">Another point on the first line.</param>
    /// <param name="b1">A point on the second line.</param>
    /// <param name="b2">Another point on the second line.</param>
    /// <returns>True, if the lines interesect, false otherwise.</returns>
    public static bool TryGetLineIntersection(Vector2 a1, Vector2 a2,
        Vector2 b1, Vector2 b2, out Vector2 intersectionPoint)
    {
        float tmp = (b2.x - b1.x) * (a2.y - a1.y) - (b2.y - b1.y) * (a2.x - a1.x);
        if (tmp == 0)
        {
            intersectionPoint = Vector2.zero;
            return false;
        }

        float mu = ((a1.x - b1.x) * (a2.y - a1.y) - (a1.y - b1.y) * (a2.x - a1.x)) / tmp;
        intersectionPoint = new Vector2(
            b1.x + (b2.x - b1.x) * mu,
            b1.y + (b2.y - b1.y) * mu
        );
        return true;
    }
}