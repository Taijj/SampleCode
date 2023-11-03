using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// <see cref="DamageReceiver"> that invokes OnKill event when health is reduced to zero.
	/// </summary>
	public class DamageReceiverCollider : DamageReceiver
	{
		#region Main
		[SerializeField] private float _health;

		public event Action OnKill;

		public override void TakeDamage(AttackInfo info)
		{
			string log = $"Took {info.Strength} damage!\n";

			_health -= info.Strength;

			if (IsDead)
				log += "I am DEAD!";
			else
				log += $"Remaining Health: {_health}";
			Note.Log(log);
		}

		public override void Kill(AttackInfo info) => OnKill?.Invoke();

		public override bool IsDead => _health <= 0;
		#endregion
	}
}
