
using UnityEngine;

public static class GizmoAddons
{
	#region Lines
	public enum LineKind
    {
        /// <summary>
        /// The line will be drawn so that the given position is in the middle of the line.
        /// </summary>
        Centered = 0,

        /// <summary>
        /// The line will be start at the given position.
        /// </summary>
        FromPosition,

        /// <summary>
        /// The line will end at the given position.
        /// </summary>
        ToPosition
    }

    /// <summary>
    /// Draws a horizontal line Gizmo at the given position, with the given length.
    /// </summary>
    public static void DrawLineHorizontal(Vector2 position, float length = 10f, LineKind kind = LineKind.Centered)
    {
        Vector2 start;
        Vector2 end;
        switch (kind)
        {
            case LineKind.FromPosition:
                start = position;
                end = position.WithX(position.x + length);
                break;

            case LineKind.ToPosition:
                start = position.WithX(position.x - length);
                end = position;
                break;

            default:
                float halfLength = length / 2f;
                start = position.WithX(position.x - halfLength);
                end = position.WithX(position.x + halfLength);
                break;
        }
        Gizmos.DrawLine(start, end);
    }

    /// <summary>
    /// Draws a vertical line Gizmo at the given position, with the given length.
    /// </summary>
    public static void DrawLineVertical(Vector2 position, float length = 10f, LineKind kind = LineKind.Centered)
    {
        Vector2 start;
        Vector2 end;
        switch (kind)
        {
            case LineKind.FromPosition:
                start = position;
                end = position.WithY(position.y + length);
                break;

            case LineKind.ToPosition:
                start = position.WithY(position.y - length);
                end = position;
                break;

            default:
                float halfLength = length / 2f;
                start = position.WithY(position.y - halfLength);
                end = position.WithY(position.y + halfLength);
                break;
        }
        Gizmos.DrawLine(start, end);
    }
	#endregion



	#region Misc
	private const float X_RADIUS = 0.1f;
	public static void DrawX(Vector2 position)
	{
		Gizmos.DrawLine(position - Vector2.one * X_RADIUS, position + Vector2.one * X_RADIUS);
		Gizmos.DrawLine(position + new Vector2(-1, 1) * X_RADIUS, position + new Vector2(1, -1) * X_RADIUS);
	}
	#endregion
}