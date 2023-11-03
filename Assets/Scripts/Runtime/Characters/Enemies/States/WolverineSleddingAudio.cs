using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Initialization State, that handles the sledding Wolverine <see cref="Enemy"/>'s
	/// audio looop lifecycle, before transitting to the actual default state.
	/// </summary>
	public class WolverineSleddingAudio : EnemyState
	{
		[SerializeField] private EventReference _slideLoop;
		[SerializeField] private EnemyState _nextState;

		public override void SetUp()
		{
			base.SetUp();

			Loop = new AudioLoop(_slideLoop, Pawn.Transform);
			Owner.OnActivationChange += OnActivationChange;
			Owner.OnDied += Loop.Stop;
		}

		public override void CleanUp()
		{
			base.CleanUp();
			Owner.OnActivationChange -= OnActivationChange;
			Owner.OnDied -= Loop.Stop;
		}		

		public void OnActivationChange(bool isActive)
		{
			if (isActive)
				Loop.Play();
			else
				Loop.Stop();
		}
				
		public override void OnUpdate() => Transit(_nextState.GetType());

		private AudioLoop Loop { get; set; }
	}
}