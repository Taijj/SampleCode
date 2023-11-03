using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Used to define areas with certain camera behavior.
	/// </summary>
	public class CameraZone : MonoBehaviour
	{
		#region Main
		[SerializeField] private MachineConfig _config;

		public void OnTriggerStay2D(Collider2D collider) => OnTriggerEnter2D(collider);
		public void OnTriggerEnter2D(Collider2D collider)
		{
			if (IsEntered)
				return;

			if (collider.TryGet(out Corpus _))
			{
				IsEntered = true;
				Level.Cameraman.Add(_config);
			}
		}

		public void OnTriggerExit2D(Collider2D collider)
		{
			if (false == IsEntered)
				return;

			if (collider.TryGet(out Corpus _))
			{
				IsEntered = false;
				Level.Cameraman.Remove(_config);
			}
		}

		private bool IsEntered { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			_config.name = name;

			if (TryGetComponent(out Collider2D collider))
				collider.isTrigger = true;
		}
		#endif
	}
}