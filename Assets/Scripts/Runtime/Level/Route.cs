
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Controller for Checkpoints and Respawning.
	/// </summary>
    public class Route : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private Checkpoint[] _checkpoints;
		[SerializeField] private Interactor[] _interactors;
		[SerializeField] private float _fadeOutDuration;
		[SerializeField] private float _fadeInDuration;

		public void Wake()
		{
			WakeRespawning();

			for(int i = 0; i < _interactors.Length; i++)
				_interactors[i].Wake();
		}

		public void SetUp() => SetUpCheckpoints();

		public void CleanUp()
		{
			for (int i = 0; i < _interactors.Length; i++)
				_interactors[i].CleanUp();
		}
		#endregion



		#region Checkpoints
		private void SetUpCheckpoints()
		{
			Positions = new Vector2[_checkpoints.Length + 1];
			Positions[0] = Hero.Pawn.OriginalPosition;
			for (int i = 0; i < _checkpoints.Length; i++)
			{
				Checkpoint point = _checkpoints[i];
				point.IsReached = false;

				Positions[i+1] = point.RespawnPosition;
			}

			CurrentIndex = 0;
		}

		public void OnReached(Checkpoint touchedCheckpoint)
		{
			for(int i = 0; i < _checkpoints.Length; i++)
			{
				Checkpoint point = _checkpoints[i];
				if(point == touchedCheckpoint)
				{
					CurrentIndex = i+1;
					point.IsReached = true;
				}
				else
				{
					point.IsReached = false;
				}
			}
		}

		private void RespawnHero() => Hero.Respawn(Positions[CurrentIndex]);

		public void TeleportHero()
		{
			CurrentIndex = (int)Mathf.Repeat(CurrentIndex+1, Positions.Length);
			Hero.Pawn.Transform.position = Positions[CurrentIndex];
		}

		private Hero Hero => Level.Hero;
		[field: SerializeField, ReadOnly] private int CurrentIndex { get; set; }
		private Vector2[] Positions { get; set; }
		#endregion



		#region Respawn
		private const int RESPAWN_METHODS_LENGTH = 30;
		public void WakeRespawning() => RespawnMethods = new Action[RESPAWN_METHODS_LENGTH];
		public void RegisterRespawn(Action method) => RespawnMethods.Add(method);
		public void DeregisterRespawn(Action method) => RespawnMethods.Remove(method);


		public void Respawn()
		{
			Game.Ui.FadeScreen(new FadeData
			{
				color = Color.black,
				duration = _fadeOutDuration,
				onCompleted = OnFadedOut
			});
		}

		private void OnFadedOut()
		{
			RespawnLevel();
			Game.Ui.FadeScreen(new FadeData
			{
				color = Color.black.With(0f),
				duration = _fadeOutDuration
			});
		}

		private void RespawnLevel()
		{
			RespawnHero();
			for (int i = 0; i < _interactors.Length; i++)
				_interactors[i].Respawn();

			for (int i = 0; i < RespawnMethods.Length; i++)
			{
				if (RespawnMethods[i] != null)
					RespawnMethods[i]();
			}
		}

		public Action[] RespawnMethods { get; private set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _checkpoints);
			this.TryAssign(ref _interactors);
		}
		#endif
	}
}