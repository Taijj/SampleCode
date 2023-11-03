using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Heater that activates when hit with any fire type, and deactivates when hit with any ice type attack
	/// </summary>
	public class Candle : DamageReceiver
	{
		#region LifeCycle
		public event Action<bool> OnCandleHit;

		[SerializeField] private Heater _heater;
		[SerializeField] private GameObject[] _effects;

		public void Wake()
		{
			_heater.Wake();
			IsBurning = false;
		}

		public void OnUpdate()
		{
			if (IsBurning)
				_heater.Heat();
		}

		public void Ignite() => IsBurning = true;
		public void Extinguish() => IsBurning = false;



		private bool _isBurning;
		private bool IsBurning
		{
			get => _isBurning;

			set
			{
				_isBurning = value;
				for (int i = 0; i < _effects.Length; i++)
					_effects[i].SetActive(_isBurning);
			}
		}
		#endregion



		#region DamageReceiver
		public override bool IsDead => false;

		public override void TakeDamage(AttackInfo info)
		{
			bool shouldIgnite = false == IsBurning && info.Attributes.IsAny(AttackAttribute.Fire);
			bool shouldExtinguish = IsBurning && info.Attributes.IsAny(AttackAttribute.Ice);

			if (shouldIgnite)
				Ignite();
			else if (shouldExtinguish)
				Extinguish();

			OnCandleHit?.Invoke(IsBurning);
		}
		#endregion
	}
}
