using Taijj.SampleCode;
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base controller for any Enemy in the game. Handles common features for all enemies.
	/// For specific behavior <see cref="EnemyState"/>s are used.
	/// </summary>
    public class Enemy : MonoBehaviour
    {
		#region LifeCycle
		[Space, Header("Parts")]
		[SerializeField] private Pawn _pawn;
		[SerializeField] private HeroDetector _heroDetector;
		[SerializeField] private CameraDetector _cameraDetector;
		[SerializeField] private DamageSender _triggerAttack;
		[Space, Header("Health")]
		[SerializeField] private Resource _health;
		[SerializeField] private Blinker _blinker;
		[SerializeField] private Blink _damageBlink;
		[SerializeField] private OreDropper _dropper;
		[Space, Header("Misc")]
		[SerializeField] private EnemyState[] _states;
		[SerializeField, ReadOnly] private bool _hasAggro;
		[SerializeField] private bool _turnsTowardsHero;

		public void Wake()
		{
			_pawn.Wake(new Pawn.PawnData
			{
				isDead = GetIsDead,
				isInvulnerable = GetIsInvul,
				isInvincible = GetIsInvincible,
				onDamaged = OnDamageTaken
			});
			_heroDetector.Wake(_pawn);
			_cameraDetector.Wake(Activate, Deactivate);

			AttackInfo info = new AttackInfo();
			info.Flinch = FlinchKind.Volume;
			_triggerAttack.Wake(info);

			WakeStates();
			_health.TopOff();
			_blinker.Wake();
			_dropper.Wake();
		}

		public void SetUp()
		{
			SetUpStates();

			_pawn.SetUp();
			_heroDetector.SetUp(Level.Hero.Pawn);
			_cameraDetector.SetUp();
		}

		public void CleanUp()
		{
			CleanUpStates();
			_pawn.CleanUp();
			_blinker.CleanUp();
		}
		#endregion



		#region Update/Active/Inactive
		private void Activate()
		{
			if (_health.IsEmpty)
				return;

			_pawn.Activate();
			_pawn.Enable();

			IsActive = true;
			OnActivationChange?.Invoke(IsActive);
		}

		private void Deactivate()
		{
			_pawn.Deactivate();
			_pawn.Disable();
			OnActivationChange?.Invoke(false);

			IsActive = false;
			OnActivationChange?.Invoke(IsActive);
		}



		public void OnUpdate()
		{
			_cameraDetector.OnUpdate();

			if(IsActive)
			{
				_triggerAttack.OnUpdate();
				CurrentState.OnUpdate();
			}
		}

		public void OnFixedUpdate()
		{
			if (IsActive)
				CurrentState.OnFixedUpdate();
		}

		public void Stop() => _pawn.Stop();

		public event Action<bool> OnActivationChange;
		private bool IsActive { get; set; }
		#endregion



		#region States
		private void WakeStates()
		{
			EnemyState.StateData data = new EnemyState.StateData
			{
				owner = this,
				pawn = _pawn,
				detector = _heroDetector,

				turnsTowardsHero = _turnsTowardsHero,
				hasAggro = GetHasAggro
			};

			for (int i = 0; i < _states.Length; i++)
			{
				EnemyState state = _states[i];

				state.Wake(data);
				state.OnTransit += TransitTo;
			}
		}

		private void SetUpStates()
		{
			for (int i = 0; i < _states.Length; i++)
				_states[i].SetUp();

			CurrentState = _states[0];
			TransitTo(_states[0].Type);
		}

		private void CleanUpStates()
		{
			for (int i = 0; i < _states.Length; i++)
			{
				_states[i].OnTransit -= TransitTo;
				_states[i].CleanUp();
			}
		}

		public void TransitTo(Type stateType)
		{
			for (int i = 0; i < _states.Length; i++)
			{
				EnemyState state = _states[i];
				if (state.Type != stateType)
					continue;

				SwitchState(state);
				return;
			}

			throw new Exception($"Couldn't find {stateType.Name} in array!");
		}

		private void SwitchState(EnemyState state)
		{
			if (CurrentState != null)
				CurrentState.Exit();
			CurrentState = state;
			CurrentState.Enter();

			#if UNITY_EDITOR
				_currentStateName = state.Type.Name;
			#endif
		}

		private EnemyState CurrentState { get; set; }
		public bool GetHasAggro() => _hasAggro;
		#endregion



		#region Health & Respawn
		public void Respawn()
		{
			_health.TopOff();
			_hasAggro = false;
			_blinker.Stop();

			_pawn.Respawn();
			_cameraDetector.Respawn();

			_states[0].gameObject.Activate();
			TransitTo(_states[0].GetType());
		}

		private void OnDamageTaken(AttackInfo info)
		{
			if (_health.IsEmpty)
				return;

			_health.Consume(info.Strength);
			if (_health.IsEmpty)
			{
				_dropper.Drop();
				Kill();
			}
			else
			{
				_hasAggro = info.SourceLayer == Layers.HERO;
				_blinker.Do(_damageBlink);
			}
		}

		private void Kill()
		{
			_pawn.Disable();

			OnDied?.Invoke();
			_states[0].gameObject.Deactivate();
			TransitTo(typeof(EnemyDead));
		}

		private bool GetIsDead() => _health.IsEmpty;
		private bool GetIsInvul() => false;
		private bool GetIsInvincible() => false;

		public event Action OnDied;
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField, ReadOnly] private string _currentStateName;

		public void OnValidate()
		{
			this.TryAssign(ref _pawn);
			this.TryAssign(ref _heroDetector);
			this.TryAssign(ref _cameraDetector);
			this.TryAssign(ref _triggerAttack);
			this.TryAssign(ref _blinker);
			this.TryAssign(ref _dropper);

			Transform states = transform.Find("States");
			if(_states.IsFaultyFixed() || states.childCount != _states.Length)
				_states = GetComponentsInChildren<EnemyState>();
		}

		[Space(30), Button(nameof(Flip))]
		public bool flipTarget;
		public void Flip() => _pawn.Flip();
		#endif
	}
}