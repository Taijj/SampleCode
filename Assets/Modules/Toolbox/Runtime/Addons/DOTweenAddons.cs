using DG.Tweening;

public static class DOTweenAddons
{
    /// <summary>
    /// Use this, if you want to reuse a Tween with different values.
    /// </summary>
    /// <param name="startValue">The value the Tween starts with, e.g. it's current value.</param>
    /// <param name="endValue">The target value the Tween will be at when done.</param>
    /// <param name="duration">OPTIONAL: Will default to the Tween's current duration, if not set.</param>
    /// <returns></returns>
    public static Tween SetValues(this Tween @this, object startValue, object endValue, float duration = -1f)
    {
        duration = duration < 0f ? @this.Duration() : duration;
        Tweener tw = (Tweener)@this;
        tw.ChangeValues(startValue, endValue, duration);
		tw.Restart();
        return @this;
    }

	/// <summary>
	/// If set to true, the Tween will ignore Unity's timescale.
	/// </summary>
	public static Tween SetIsTimeScaleIgnored(this Tween @this, bool value) => @this.SetUpdate(value);
}