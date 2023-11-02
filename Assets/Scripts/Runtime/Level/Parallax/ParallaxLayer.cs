using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Layer that moves similar to the Camera with its speed depending on the parallax Effect Multiplier and the Z position of its transform.
	/// </summary>
	public class ParallaxLayer : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField, ReadOnly] private Transform _transform;		
		[SerializeField, ReadOnly] private float _depthMultiplier;

		public void Wake() => Transform = transform;

		public void UpdateParallax(Vector2 moveDelta)
		{
			Transform.position += (Vector3)moveDelta * _depthMultiplier;
		}

		private Transform Transform { get; set; }
		#endregion



		#if UNITY_EDITOR
		[SerializeField, HideInInspector] private FloatRange _range;

		public void Validate(FloatRange range)
		{
			_range = range;
			Transform = transform;

			float z = Transform.position.z;
			Transform.position = new Vector3(0f, 0f, z);
			_depthMultiplier = GetDepthMultiplier(z);
		}

		public float GetDepthMultiplier(float z)
		{			
			if(z >= 0)
				return Mathf.Clamp(z/_range.Max, 0f, 1f);
			else
				return Mathf.Clamp(-Mathf.Abs(z/_range.Min), -1f, 0f);
		}
		#endif
	}
}
