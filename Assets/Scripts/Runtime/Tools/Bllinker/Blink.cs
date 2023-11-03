using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Used to define a certain, reusable blink behavior.
	/// </summary>
    [CreateAssetMenu(fileName = "Blink", menuName = "Ble/Blink")]
    public class Blink : ScriptableObject
    {
		#region Configuration
		[ColorUsage(true, true)]
		public Color color = Color.white;
		public float duration = 0.1f;
		public AnimationCurve pattern;
		public bool isLooping;
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if (pattern == null || pattern.length == 0)
				pattern = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}
		#endif
	}
}