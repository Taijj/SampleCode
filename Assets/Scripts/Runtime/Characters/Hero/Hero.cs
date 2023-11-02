using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Controller and manager for the "Player" or "Avatar". The main
	/// protagonist(s) of this game.
	/// </summary>
	public class Hero : MonoBehaviour
	{
		#region LifeCycle
		[Space, Header("Parts")]
		[SerializeField] private PawnPhysical _pawn;
		[SerializeField] private HeroArsenal _arsenal;
		[SerializeField] private InputProvider _input;
		[SerializeField] private Transform _cameraAnchor;
		[Space, Header("Logical")]
		[SerializeField] private HeroState[] _states;
		[SerializeField] private HeroInvul _invul;
		[SerializeField] private Resource _health;
		[SerializeField] private Resource _mana;
		[Space, Header("Misc")]
		public bool godMode;

		public HeroArsenal Arsenal { get => _arsenal; set => _arsenal = value; }
		public InputProvider InputProvider { get => _input; set => _input = value; }

		public void Wake()
		{
			_pawn.Wake(new Pawn.PawnData
			{
				isDead = GetIsDead,
				isInvulnerable = GetIsInvul,
				isInvincible = GetIsInvincible,
				onDamaged = OnDamageTaken
			});
			_arsenal.Wake(new HeroArsenal.ArsenalData
			{
				mana = _mana,
				transitToState = TransitTo,
				throwBomb = ThrowBomb
			});
			_health.TopOff();
			_mana.TopOff();
			_invul.Wake();

			WakeStates();
			godMode = false;
		}

		public void SetUp()
		{
			_pawn.SetUp();
			_input.Enable();

			_pawn.Animator.OnEvent += OnAnimatorEvent;

			Game.Updater.AddUpdate(OnUpdate);
			Game.Updater.AddFixed(OnFixedUpdate);
		}

		public void Respawn(Vector2 position)
		{
			_pawn.Stop();
			_pawn.Respawn();
			_pawn.Activate();
			_pawn.Enable();

			_health.TopOff();
			_mana.TopOff();
			_invul.Respawn();

			Interactor = null;

			_pawn.Transform.position = position;
			TransitTo(_states[0].GetType());
		}

		public void CleanUp()
		{
			Game.Updater.RemoveUpdate(OnUpdate);
			Game.Updater.RemoveFixed(OnFixedUpdate);

			_pawn.Animator.OnEvent -= OnAnimatorEvent;

			CleanUpStates();
			_input.Disable();
			_pawn.CleanUp();
			_arsenal.CleanUp();
			_invul.CleanUp();
		}

		public PawnPhysical Pawn => _pawn;
		public Resource Health => _health;
		public Resource Mana => _mana;
		public Transform CameraAnchor => _cameraAnchor;

		public Interactor Interactor { set => _input.Set(value); }
		#endregion



		#region States
		private void WakeStates()
		{
			HeroState.StateData data = new HeroState.StateData
			{
				pawn = _pawn,
				arsenal = _arsenal,
				input = _input,
				invul = _invul
			};

			for (int i = 0; i < _states.Length; i++)
			{
				HeroState state = _states[i];
				if (state is HeroFlinching)
					Flinch = state as HeroFlinching;

				state.Wake(data);
				state.OnTransit += TransitTo;
			}

			TransitTo(typeof(HeroMoving));
		}

		private void CleanUpStates()
		{
			for (int i = 0; i < _states.Length; i++)
			{
				_states[i].OnTransit -= TransitTo;
				_states[i].CleanUp();
			}
		}

		private void OnUpdate()
		{
			if (_health.IsEmpty)
				return;

			CurrentState.OnUpdate();
		}

		private void OnFixedUpdate()
		{
			if (_health.IsEmpty)
				return;

			CurrentState.OnFixedUpdate();
		}


		public void TransitTo(Type stateType)
		{
			for(int i = 0; i < _states.Length; i++)
			{
				HeroState state = _states[i];
				if (state.Type != stateType)
					continue;

				SwitchState(state);
				return;
			}

			throw new Exception($"Couldn't find {stateType.Name} in array!");
		}

		private void SwitchState(HeroState state)
		{
			if (CurrentState != null)
				CurrentState.Exit();
			CurrentState = state;
			CurrentState.Enter();

			#if UNITY_EDITOR
				_currentStateName = state.Type.Name;
			#endif
		}

		private HeroState CurrentState { get; set; }
		#endregion



		#region Health & Mana
		private void OnDamageTaken(AttackInfo info)
		{
			if (godMode)
				return;

			_health.Consume(info.Strength);
			if(false == _health.IsEmpty)
			{
				Flinch.Info = info;
				TransitTo(Flinch.Type);
			}
			else
			{
				Kill();
			}
		}

		public void Kill()
		{
			_arsenal.IsBreathingFire = false;
			_arsenal.IsBreathingIce = false;

			_health.EmptyOut();
			_pawn.Disable();
			TransitTo(typeof(HeroDead));
		}

		public void Heal(float amount)
		{
			_invul.BlinkHeal();
			_health.Add(amount);
		}

		public void Recharge(float mana) => _mana.Add(mana);

		public bool GetIsDead() => _health.IsEmpty;
		private bool GetIsInvul() => _invul.IsInvulnerable;
		private bool GetIsInvincible() => CurrentState.Type == typeof(HeroDashing)
							|| CurrentState.Type == typeof(HeroStomping);

		private HeroFlinching Flinch { get; set; }
		#endregion



		#region Bombs
		private void ThrowBomb(Mortar mortar)
		{
			CurrentBombMortar = mortar;
			Pawn.Animator.Trigger(AnimatorHashes.THROW);
		}

		private void OnAnimatorEvent(CharacterAnimator.Event e)
		{
			if(e == CharacterAnimator.Event.Default)
			{
				CurrentBombMortar.TryPerform();
				CurrentBombMortar.Cease();
			}
		}

		private Mortar CurrentBombMortar { get; set; }
		#endregion


		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField, ReadOnly] private string _currentStateName;


		public void OnValidate()
		{
			this.TryAssign(ref _pawn);
			this.TryAssign(ref _input);
			this.TryAssign(ref _arsenal);

			this.TryAssign(ref _invul);

			HeroState[] states = GetComponentsInChildren<HeroState>();
			if (_states.IsFaultyFixed() || states.Length != _states.Length)
				_states = states;
		}



		[Space(30), Button(nameof(Flip))]
		public bool flipTarget;
		public void Flip() => _pawn.Flip();
		#endif
	}
}
