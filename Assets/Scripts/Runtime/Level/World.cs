using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Manager for mechanics of the game world.
	/// </summary>
    public class World : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private SnowBox[] _snowBoxes;
		[SerializeField] private SnowBall[] _snowBalls;
		[Space]
		[SerializeField] private MeteorShard[] _meteorShards;
		[SerializeField] private IceShard[] _iceShards;
		[Space]
		[SerializeField] private ParallaxArea _parallaxArea;

		public void Wake()
		{
			for (int i = 0; i < _snowBoxes.Length; i++)
				_snowBoxes[i].Wake();
			for (int i = 0; i < _snowBalls.Length; i++)
				_snowBalls[i].Wake();

			for (int i = 0; i < _meteorShards.Length; i++)
				_meteorShards[i].Wake();
			for (int i = 0; i < _iceShards.Length; i++)
				_iceShards[i].Wake();

			HasParallax = _parallaxArea.HasReference(true);
			if(HasParallax)
				_parallaxArea.Wake();
		}

		public void SetUp()
		{
			for (int i = 0; i < _iceShards.Length; i++)
				_iceShards[i].SetUp();

			if (HasParallax)
				_parallaxArea.SetUp();

			Level.Route.RegisterRespawn(OnRespawn);
			Game.Updater.AddUpdate(OnUpdate);
			Game.Updater.AddFixed(OnFixedUpdate);
			Game.Updater.AddLate(OnLateUpdate);
		}

		public void CleanUp()
		{
			for (int i = 0; i < _meteorShards.Length; i++)
				_meteorShards[i].CleanUp();

			for (int i = 0; i < _snowBalls.Length; i++)
				_snowBalls[i].CleanUp();

			Game.Updater.RemoveUpdate(OnUpdate);
			Game.Updater.RemoveFixed(OnFixedUpdate);
			Game.Updater.RemoveLate(OnLateUpdate);
			Level.Route.DeregisterRespawn(OnRespawn);
		}

		private bool HasParallax { get; set; }
		#endregion



		#region Flow
		private void OnUpdate()
		{
			for (int i = 0; i < _meteorShards.Length; i++)
				_meteorShards[i].OnUpdate();
		}

		private void OnFixedUpdate()
		{
			for (int i = 0; i < _snowBalls.Length; i++)
				_snowBalls[i].OnFixedUpdate();
		}

		private void OnLateUpdate()
		{
			for (int i = 0; i < _snowBoxes.Length; i++)
				_snowBoxes[i].ApplyChanges();

			if (HasParallax)
				_parallaxArea.OnLateUpdate();
		}



		private void OnRespawn()
		{
			for (int i = 0; i < _snowBalls.Length; i++)
				_snowBalls[i].Respawn();
			for (int i = 0; i < _snowBoxes.Length; i++)
				_snowBoxes[i].Respawn();

			for (int i = 0; i < _meteorShards.Length; i++)
				_meteorShards[i].Respawn();
			for (int i = 0; i < _iceShards.Length; i++)
				_iceShards[i].Respawn();
		}
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _snowBoxes);
			this.TryAssign(ref _snowBalls);
			this.TryAssign(ref _meteorShards);
			this.TryAssign(ref _iceShards);
			this.TryAssign(ref _parallaxArea);
		}
		#endif
	}
}