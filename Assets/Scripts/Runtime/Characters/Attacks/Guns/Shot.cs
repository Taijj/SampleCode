using FMODUnity;
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// A projectile, in different states:
	/// - None: Not active, invisible
	/// - Alive: Active, flying unhindered
	/// - Dissolving: Active, Dissolving in water
	/// - Disintegrating: Inactive, exploding visually
	/// </summary>
	[RequireComponent(typeof(DamageSender))]
	public class Shot : MonoBehaviour, IWaterlogged
	{
		#region LifeCycle
		[Space, Header("Parts")]
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private DamageSender _damageSender;
		[SerializeField] private Animator _animator;
		[SerializeField] private Collider2D[] _colliders;
		[Space, Header("SnowBox")]
		[SerializeField] private Heater _heater;
		[SerializeField] private float _waterVelocityModifier;
		[SerializeField] private float _dissolveDuration;
		[Space, Header("Explosion")]
		[SerializeField] private bool _shouldExplodeOnHit;
		[DrawIf(nameof(_shouldExplodeOnHit), true)]
		[SerializeField] private DamageSender _explosionDamageSender;
		[DrawIf(nameof(_shouldExplodeOnHit), true)]
		[Space, Header("Audio")]
		[SerializeField] private EventReference _spawnSound;
		[SerializeField] private EventReference _hitSound;
		[SerializeField] private EventReference _missSound;
		[Space, Header("Misc")]
		[DrawIf(nameof(_rotateAroundSelf), false)]
		[SerializeField] private bool _rotateByVelocity;
		[DrawIf(nameof(_rotateByVelocity), false)]
		[SerializeField] private bool _rotateAroundSelf;
		[DrawIf(nameof(_rotateAroundSelf), true)]
		[SerializeField] private float _rotationSpeed;

		private Transform Transform { get; set; }
		private AttackInfo AttackInfo { get; set; }
		private AttackInfo ExplosionAttackInfo { get; set; }
		public bool IsReady { get; private set; }
		
		


		public void Wake()
		{
			Transform = transform;

			Cam = Level.Cameraman.Camera;

			AttackInfo = new AttackInfo();
			_damageSender.Wake(AttackInfo);
			_damageSender.OnDamageSent += OnDamageSent;

			if (_shouldExplodeOnHit)
			{
				ExplosionAttackInfo = new AttackInfo
				{
					Flinch = FlinchKind.Volume
				};
				_explosionDamageSender.Wake(ExplosionAttackInfo);
				_explosionDamageSender.Deactivate();
			}

			_heater.Wake();

			OriginalRigidbodyType = _rigidbody.bodyType;
			Disable();
			IsReady = true;
		}

		public void CleanUp()
		{
			_damageSender.OnDamageSent -= OnDamageSent;
			CurrentState = State.None;
		}



		private void Enable()
		{
			_rigidbody.bodyType = OriginalRigidbodyType;
			for (int i = 0; i < _colliders.Length; i++)
				_colliders[i].enabled = true;
		}

		private void Disable()
		{
			for (int i = 0; i < _colliders.Length; i++)
				_colliders[i].enabled = false;

			if(_rigidbody.bodyType != RigidbodyType2D.Static)
			{
				Velocity = Vector2.zero;
				_rigidbody.bodyType = RigidbodyType2D.Static;
			}
		}

		private RigidbodyType2D OriginalRigidbodyType { get; set; }
		#endregion



		#region State
		private enum State
		{
			None = 0,

			Alive,
			Dissolving,
			Disintegrating
		}

		public void OnUpdate()
		{
			switch (CurrentState)
			{
				case State.Dissolving: UpdateDissolving(); break;
				case State.Disintegrating: UpdateDisintegration(); break;
				case State.Alive: UpdateAlive(); break;
			}
		}

		private State CurrentState { get; set; }
		#endregion



		#region Alive
		public void Shoot(ShotInfo info)
		{
			Transform.position = info.position;
			_rigidbody.position = info.position;
			Enable();

			OffscreenTolerance = info.offscreenTolerance;
			Velocity = info.velocity;

			AttackInfo.Direction = info.velocity;
			OnDestroyed = info.onDestroyed;
			_animator.Reset();

			Game.Audio.Play(_spawnSound, Transform);
			CurrentState = State.Alive;
		}

		private void UpdateAlive()
		{
			if(_rotateByVelocity)
			{
				Vector2 vel = _rigidbody.velocity;
				float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
				Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}

			if (_rotateAroundSelf)
			{
				float angle = _rotationSpeed * -Mathf.Sign(AttackInfo.Direction.x);
				Transform.Rotate(Vector3.forward, angle, Space.World);
			}

			if (false == IsOffscreen())
				_damageSender.OnUpdate();
			else
				Destroy();
		}

		private void OnDamageSent(DamageOutcome outcome)
		{
			if (outcome.kind == DamageOutcome.Kind.Ignored)
				return;

			bool isHeatableOutcome = outcome.kind.IsEither(DamageOutcome.Kind.Damaged, DamageOutcome.Kind.HitSolid);
			if (isHeatableOutcome)
				_heater.Heat();

			if (_shouldExplodeOnHit)
			{
				_explosionDamageSender.Activate();
				_explosionDamageSender.OnUpdate();
			}

			Game.Audio.Play(outcome.kind == DamageOutcome.Kind.Damaged ? _hitSound : _missSound, Transform);
			Disintegrate();
		}

		private Vector2 Velocity
		{
			set
			{
				_rigidbody.velocity = value;
				Transform.SetFacing(_rigidbody.velocity);
			}
		}
		#endregion



		#region Dissolving/Water
		public void OnEnterWater(MoveData.Modifiers _)
		{
			_rigidbody.velocity *= _waterVelocityModifier;

			DissolvedTime = Time.time + _dissolveDuration;
			CurrentState = State.Dissolving;
		}

		private void UpdateDissolving()
		{
			UpdateAlive();

			if (Time.time > DissolvedTime)
			{
				_heater.Heat();
				Disintegrate();
			}
		}

		public void OnExitWater() {}

		private float DissolvedTime { get; set; }
		#endregion



		#region Disintegration
		private static readonly int DISINTEGRATE_HASH = Animator.StringToHash("Disintegrate");

		public void Disintegrate()
		{
			Disable();

			_animator.TriggerInstantly(DISINTEGRATE_HASH, out float clipLength);
			if(_shouldExplodeOnHit)
			{
				// Show Explosion Vfx

				Level.Cameraman.Shake.Do(CameraShake.Weak);
			}
			DisintegratedTime = Time.time + clipLength;
			CurrentState = State.Disintegrating;
		}

		private void UpdateDisintegration()
		{
			if (_shouldExplodeOnHit)
				return;

			if (Time.time > DisintegratedTime)
				Destroy();
		}

		private float DisintegratedTime { get; set; }
		#endregion



		#region Misc
		public void Destroy()
		{
			CurrentState = State.None;
			if (_shouldExplodeOnHit)
				_explosionDamageSender.Deactivate();

			Disable();
			OnDestroyed.Invoke(this);
		}

		public event Action<Shot> OnDestroyed;
			

		private bool IsOffscreen()
		{
			Vector3 pos = Cam.WorldToViewportPoint(Transform.position);
			return pos.x < -OffscreenTolerance
				|| pos.x > 1f + OffscreenTolerance
				|| pos.y < -OffscreenTolerance
				|| pos.y > 1f + OffscreenTolerance;
		}

		private Camera Cam { get; set; }
		private float OffscreenTolerance { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if(_rigidbody.IsNull(true)) _rigidbody = GetComponent<Rigidbody2D>();
			if(_damageSender.IsNull(true)) _damageSender = GetComponentInChildren<DamageSender>();
			if(_animator.IsNull(true)) _animator = GetComponentInChildren<Animator>();
			if (_colliders.IsFaultyFixed()) _colliders = GetComponentsInChildren<Collider2D>();
		}
		#endif
	}
}