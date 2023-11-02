using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// The visual FireDash "attack" (NOTE: not really an <see cref="Attack"/>)
	/// Handles damaging and heating.
	/// </summary>
    public class FireDash : MonoBehaviour
    {
		#region Main
		[SerializeField] private DamageSender _damager;
		[SerializeField] private Heater _heater;

		public void Wake(Action forceRecoil)
		{
			ForceRecoil = forceRecoil;

			Condition = new SpecialCondition();
			AttackInfo info = new AttackInfo();
			info.AddAttribute(AttackAttribute.Fire);
			_damager.Wake(info, Condition);
			_damager.OnDamageSent += OnDamaged;

			_heater.Wake();
			this.Deactivate();
		}

		private void OnDamaged(DamageOutcome outcome)
		{
			if (outcome.receiver is IceShard)
				return;

			if (outcome.kind.IsEither(DamageOutcome.Kind.HitSolid, DamageOutcome.Kind.Damaged))
				ForceRecoil();
		}

		public void CleanUp()
		{
			_damager.OnDamageSent -= OnDamaged;
			this.Deactivate();
		}

		private Action ForceRecoil { get; set; }
		#endregion



		#region Flow
		public void Begin()
		{
			Condition.Clear();
			this.Activate();
		}

		public void OnUpdate()
		{
			_damager.OnUpdate();
			_heater.Heat();
		}

		public void End() => this.Deactivate();

		private SpecialCondition Condition { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _damager);
			_heater.enabled = true;
		}
		#endif
	}
}