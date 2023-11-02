using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Top Level manager for all <see cref="Enemy"/>s and <see cref="Npc"/>s.
	/// </summary>
	public class Horde : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private Enemy[] _enemies;
		[SerializeField] private Npc[] _npcs;

		public void Wake()
		{
			for (int i = 0; i < _enemies.Length; i++)
				_enemies[i].Wake();
			for (int i = 0; i < _npcs.Length; i++)
				_npcs[i].Wake();
		}

		public void SetUp()
		{
			for (int i = 0; i < _enemies.Length; i++)
				_enemies[i].SetUp();
			for (int i = 0; i < _npcs.Length; i++)
				_npcs[i].SetUp();

			Level.Route.RegisterRespawn(OnRespawn);
			Game.Updater.AddUpdate(OnUpdate);
			Game.Updater.AddFixed(OnFixedUpdate);
		}

		public void CleanUp()
		{
			for (int i = 0; i < _enemies.Length; i++)
				_enemies[i].CleanUp();
			for (int i = 0; i < _npcs.Length; i++)
				_npcs[i].CleanUp();

			Game.Updater.RemoveUpdate(OnUpdate);
			Game.Updater.RemoveFixed(OnFixedUpdate);
			Level.Route.DeregisterRespawn(OnRespawn);
		}
		#endregion



		#region Flow
		private void OnUpdate()
		{
			if (IsPaused)
				return;

			for (int i = 0; i < _enemies.Length; i++)
				_enemies[i].OnUpdate();
			for (int i = 0; i < _npcs.Length; i++)
				_npcs[i].OnUpdate();
		}

		private void OnFixedUpdate()
		{
			for (int i = 0; i < _enemies.Length; i++)
			{
				if (IsPaused)
					_enemies[i].Stop();
				else
					_enemies[i].OnFixedUpdate();
			}
			for (int i = 0; i < _npcs.Length; i++)
			{
				if (IsPaused)
					_npcs[i].Stop();
				else
					_npcs[i].OnFixedUpdate();
			}
		}



		private void OnRespawn()
		{
			for (int i = 0; i < _enemies.Length; i++)
				_enemies[i].Respawn();
			for (int i = 0; i < _npcs.Length; i++)
				_npcs[i].Respawn();

			IsPaused = false;
		}

		public bool IsPaused { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			_enemies = GetComponentsInChildren<Enemy>();
			_npcs = GetComponentsInChildren<Npc>();
		}
		#endif
	}
}