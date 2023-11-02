using Cinemachine;
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// The visual IceStomp "attack" (NOTE: not really an <see cref="Attack"/>)
	/// Handles damaging and heating.
	/// </summary>
	public class IceStomp : MonoBehaviour
    {
		#region Main
		private static readonly int IMPACT_HASH = Animator.StringToHash("Impact");

		[SerializeField] private DamageSender _damager;
		[SerializeField] private Heater _heater;
		[SerializeField] private Animator _animator;		
		[SerializeField] private CinemachineImpulseSource _impulse;

		public void Wake(Action onEnemyHit)
		{
			OnEnemyHit = onEnemyHit;
			Call = new DelayedCall(Complete);
						
			AttackInfo info = new AttackInfo();
			_damager.Wake(info);
			_damager.OnDamageSent += OnDamaged;

			_heater.Wake();
			this.Deactivate();
		}

		private void OnDamaged(DamageOutcome outcome)
		{
			if (outcome.kind != DamageOutcome.Kind.Damaged)				
				return;

			if (outcome.receiver.gameObject.layer == Layers.ENEMY)
				OnEnemyHit();
		}

		public void CleanUp()
		{
			_damager.OnDamageSent -= OnDamaged;
			Call.Stop();
			this.Deactivate();
		}		

		private Action OnEnemyHit { get; set; }
		#endregion



		#region Flow
		public void Begin()
		{
			this.Activate();
			_animator.Reset();
			Call.Stop();
		}

		public void UpdateDamager() => _damager.OnUpdate();

		public void Impact()
		{
			_impulse.GenerateImpulse();
			_animator.TriggerInstantly(IMPACT_HASH, out float length);

			_heater.Heat();
			Call.Restart(length);
		}

		private void Complete()
		{
			_animator.Reset();
			this.Deactivate();
		}

		private DelayedCall Call { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _damager);
			this.TryAssign(ref _animator);
			this.TryAssign(ref _impulse);
			_heater.enabled = true;
		}
		#endif
	}
}