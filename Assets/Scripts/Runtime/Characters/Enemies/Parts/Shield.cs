
using FMODUnity;
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Shield used by the Wolverine <see cref="Enemy"/>. Manages its own health and visuals.
	/// </summary>
	public class Shield : DamageReceiver
	{
		#region LifeCycle
		[Space, Header("Shield")]
		[SerializeField] private Collider2D _collider;
		[SerializeField] private GameObject[] _stateObjects;
		[SerializeField] private GameObject[] _additionalObjects;
		[SerializeField] private Resource _health;
		[Space]
		[SerializeField] private Blinker _blinker;
		[SerializeField] private Blink _damageBlink;
		[SerializeField] private Blink _healBlink;
		[Space]
		[SerializeField] private EventReference _damageSound;
		[SerializeField] private EventReference _healSound;
		[SerializeField] private EventReference _dieSound;

		public void Wake(Action onDied)
		{
			Transform = transform;
			OnDied = onDied;

			_health.TopOff();
			_blinker.Wake();
			UpdateVisuals();
		}

		public void Respawn()
		{
			_blinker.Stop();

			_collider.enabled = true;
			_health.TopOff();
			UpdateVisuals();
		}

		public void CleanUp() => _blinker.CleanUp();

		private Transform Transform { get; set; }
		private Action OnDied { get; set; }
		public override bool IsDead => _health.IsEmpty;
		#endregion



		#region Damage
		public override void TakeDamage(AttackInfo info)
		{
			bool isDamage = info.Attributes.IsAny(AttackAttribute.Fire);
			bool isHeal = info.Attributes.IsAny(AttackAttribute.Ice);
			if (!(isDamage || isHeal))
				return;

			bool isAlive = true;
			if (isDamage)
				Damage(info.Strength, out isAlive);
			else
				Heal(info.Strength);

			UpdateVisuals(isAlive);
		}

		private void Damage(float amount, out bool isStillAlive)
		{
			_health.Consume(amount);
			if (!_health.IsEmpty)
			{
				_blinker.Do(_damageBlink);
				isStillAlive = true;

				Game.Audio.Play(_damageSound, Transform);
			}
			else
			{
				_collider.enabled = false;
				isStillAlive = false;

				Game.Audio.Play(_dieSound, Transform);
				OnDied();
			}
		}

		private void Heal(float amount)
		{
			_health.Add(amount);
			_blinker.Do(_healBlink);

			Game.Audio.Play(_healSound, Transform);
		}

		private void UpdateVisuals(bool isAlive = true)
		{
			for(int i = 0; i < _additionalObjects.Length; i++)
				_additionalObjects[i].SetActive(isAlive);

			float frac = (1f-_health.Normalized) * _stateObjects.Length;
			int fracInt = Mathf.FloorToInt(frac);
			for(int i = 0; i < _stateObjects.Length; i++)
				_stateObjects[i].SetActive(isAlive && i == fracInt);
		}
		#endregion



		#if UNITY_EDITOR
		public void OnValidate() => this.TryAssign(ref _collider);
		#endif
	}
}