using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// A <see cref="Pickup"/> that reappears after the set delay,
	/// when it was collected.
	/// </summary>
    public class PickupReappearing : Pickup
    {
		#region LifeCycle Basic
		[Space, Header("Reappering")]
		[SerializeField] private float _reappearDelay;
		[SerializeField] private EventReference _reappearSound;

		public override void Wake()
		{
			base.Wake();
			Call = new DelayedCall(Reapper, _reappearDelay);
		}


		public override void CleanUp()
		{
			base.CleanUp();
			Call.Stop();
		}

		private DelayedCall Call { get; set; }
		#endregion



		#region LifeCycle Collect
		protected override void Collect()
		{
			base.Collect();
			Call.Restart();
		}

		private void Reapper()
		{
			Game.Audio.Play(_reappearSound, Transform);
			base.Respawn();
		}

		public override void Respawn()
		{
			Call.Stop();
			base.Respawn();
		}
		#endregion
	}
}