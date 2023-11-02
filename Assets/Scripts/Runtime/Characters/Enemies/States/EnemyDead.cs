using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State that handles <see cref="Enemy"/> death.
	/// </summary>
    public class EnemyDead : EnemyState
    {
		[SerializeField] private EventReference _sound;

		public override void Wake(StateData data)
		{
			base.Wake(data);
			// Wake Vfx
		}

		public override void CleanUp()
		{
			base.CleanUp();
            // Clean Vfx up
		}

		public override void Enter()
		{
			Game.Audio.Play(_sound, Pawn.Transform);
			Pawn.Stop();
			Pawn.Deactivate();
			
            // Play Vfx
		}		
	}
}
