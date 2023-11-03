using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Object for testing Interactors.
	/// </summary>
    public class DummyInteractor : Interactor
    {
		[SerializeField] private float _delay;

		protected override void Interact()
		{
			Note.Log("Start!", ColorAddons.Yellow);
			DelayedCall call = new DelayedCall(OnCompleted, _delay);
			call.Start();
		}

		private void OnCompleted()
		{
			Note.Log("Complete!", ColorAddons.Lime);
			Complete();
		}
	}
}