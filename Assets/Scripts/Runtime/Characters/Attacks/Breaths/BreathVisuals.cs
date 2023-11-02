
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Controller of a <see cref="Breath"/>'s graphics, especially
	/// particles.
	/// </summary>
	[System.Serializable]
    public class BreathVisuals
    {
		#region Visibility
		[SerializeField] private GameObject _container;
		[SerializeField] private ParticleSystem[] _systems;
		[SerializeField] private ParticleSystemRenderer _cloudsRenderer;

		public void Show()
		{
			_container.Activate();
			RefreshParticles();
		}

		public void Hide() => _container.Deactivate();
		#endregion



		#region Update
		public void OnUpdate(int facing)
		{
			bool needsRefresh = facing != _lastFacing;
			_lastFacing = facing;

			if (needsRefresh)
				RefreshParticles();
		}

		private void RefreshParticles()
		{
			for (int i = 0; i < _systems.Length; i++)
			{
				ParticleSystem system = _systems[i];
				system.Stop(true);
				system.Clear(true);
				system.Play(true);
			}

			int flip = _lastFacing == 1 ? 0 : 1;
			_cloudsRenderer.flip = Vector2.right * flip;
		}

		private int _lastFacing;
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if (_container.IsNull(true))
				return;

			if (_systems.IsFaultyFixed()) _systems = _container.GetComponentsInChildren<ParticleSystem>();
			if (_cloudsRenderer.IsNull(true)) _cloudsRenderer = _container.transform.Find("Clouds").GetComponent<ParticleSystemRenderer>();
		}
		#endif
	}
}