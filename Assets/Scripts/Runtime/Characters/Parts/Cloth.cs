using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Taijj.SampleCode
{
	/// <summary>
	/// A physical component consisting of multiple joints chained
	/// together. In order for this to work properly, it has to update
	/// its position and velocity manually.
	/// </summary>
	public class Cloth : CharacterSkin
	{
		#region LifeCycle
		[Space, Header("Cloth")]
		[SerializeField] Transform _anchor;
		[SerializeField] Rigidbody2D _targetRigidbody;
		[SerializeField] Rigidbody2D _ownRigidbody;
		[Space]
		[SerializeField] private float _idleWindStrength = 1;
		[SerializeField] private float _idleWindAmplitude = 1;
		[SerializeField] private float _idleWindFrequency = 1;

		public void Wake() => Transform = transform;
		public void SetUp() => Game.Updater.AddFixed(OnUpdate);
		public void CleanUp() => Game.Updater.RemoveFixed(OnUpdate);
		#endregion



		#region Update
		private void OnUpdate()
		{
			if (_targetRigidbody.velocity == Vector2.zero)
				SimulateIdleWind();
			else
				SimulateMovingWind();

			Transform.position = (Vector2)_anchor.position;
		}

		private void SimulateIdleWind()
		{
			float sine = _idleWindAmplitude * Mathf.Sin(_idleWindFrequency * Time.time);
			_ownRigidbody.velocity = Vector2.right * (_idleWindStrength + sine);
		}

		private void SimulateMovingWind()
		{
			_ownRigidbody.velocity = _targetRigidbody.velocity;
		}

		private Transform Transform { get; set; }
		#endregion



		#if UNITY_EDITOR
		new public void OnValidate()
		{
			base.OnValidate();
			this.TryAssign(ref _ownRigidbody);
		}
		#endif
	}
}
