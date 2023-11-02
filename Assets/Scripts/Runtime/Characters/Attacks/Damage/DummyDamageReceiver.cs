
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Dummy entity for testing, if damage is properly received.
	/// </summary>
	public class DummyDamageReceiver : DamageReceiver
	{
		#region Main
		[SerializeField] private float _health;

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

		public override void Kill(AttackInfo info) => gameObject.SetActive(false);

		private void SetHealth(float value)
		{
			_health = value;
			gameObject.SetActive(false == IsDead);
		}

		public override bool IsDead => _health <= 0;
		#endregion



		#if UNITY_EDITOR
		[Space]
		[Button("SetTo0")] public bool buttonTarget0;
		[Button("SetTo5")] public bool buttonTarget5;
		[Button("SetTo10")] public bool buttonTarget10;
		[Button("SetTo20")] public bool buttonTarget20;

		public void SetTo0() => SetHealth(0);
		public void SetTo5() => SetHealth(5);
		public void SetTo10() => SetHealth(10);
		public void SetTo20() => SetHealth(20);
		#endif
	}
}