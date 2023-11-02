using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Manager and controller for the invulnerability period after
	/// the <see cref="Hero"/> took a hit.
	/// </summary>
    public class HeroInvul : MonoBehaviour
    {
		#region LifeCycle
		[Space, Header("Blink & Invul")]
		[SerializeField] private Blinker _blinker;
		[SerializeField] private Blink _damageBlink;
		[SerializeField] private Blink _invulBlink;
		[SerializeField] private Blink _healBlink;
		[Space]
		[SerializeField] private float _invulDuration;

		public void Wake()
		{
			_blinker.Wake();
			Call = new DelayedCall(Stop);
		}

		public void CleanUp()
		{
			_blinker.CleanUp();
			Call.Stop();
		}

		public void Respawn() => Stop();
		#endregion



		#region Blinks
		public void BlinkDamage()
		{
			IsInvulnerable = true;
			_blinker.Do(_damageBlink);
		}

		public void BlinkHeal()
		{
			IsInvulnerable = false;
			_blinker.Do(_healBlink);
		}
		#endregion



		#region Invul		
		public void StartInvul()
		{
			IsInvulnerable = true;
			_blinker.Do(_invulBlink);

			Call.Restart(_invulDuration);
		}

		public void StartHiddenInvul(float duration)
		{
			IsInvulnerable = true;
			Call.Restart(duration);
		}

		private void Stop()
		{
			Call.Stop();
			_blinker.Stop();
			IsInvulnerable = false;
		}

		private DelayedCall Call { get; set; }
		public bool IsInvulnerable { get; private set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate() => this.TryAssign(ref _blinker);
		#endif
	}
}
