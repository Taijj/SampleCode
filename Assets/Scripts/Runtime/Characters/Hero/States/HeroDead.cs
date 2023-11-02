using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State that handles <see cref="Hero"/> death.
	/// </summary>
	public class HeroDead : HeroState
	{
		[SerializeField] private EventReference _sound;

		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Disable();
			Pawn.Animator.Trigger(AnimatorHashes.DIE);

			Game.Audio.Play(_sound, Pawn.Transform);
			Level.Route.Respawn();
		}
	}
}