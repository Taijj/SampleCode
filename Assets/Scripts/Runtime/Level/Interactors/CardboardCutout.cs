using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
    public class CardboardCutout : HintTotem
    {
		[Space, Header("Cardboard")]
		[SerializeField] private Animator _cardboardAnimator;
		[SerializeField] private EventReference _cardboardAudio;

		public override void CleanUp() => Delay?.Stop();

		protected override void Interact()
		{
			if (HasTumbled)
			{
				base.Interact();
				return;
			}

			HasTumbled = true;
			_cardboardAnimator.TriggerInstantly(TUMBLE_HASH, out float length);
			Game.Audio.Play(_cardboardAudio);
			Delay = new DelayedCall(base.Interact, length+1f);
			Delay.Start();
		}

		private static readonly int TUMBLE_HASH = Animator.StringToHash("Tumble");
		private DelayedCall Delay { get; set; }
		private bool HasTumbled { get; set; }
	}
}