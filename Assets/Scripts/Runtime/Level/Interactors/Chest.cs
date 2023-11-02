using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// An <see cref="Interactor"/> that drops some ore.
	/// </summary>
    public class Chest : Interactor
    {
		#region Main
		[Space, Header("Chest")]
		[SerializeField] private Animator _animator;
		[SerializeField] private OreDropper _dropper;
		[SerializeField] private EventReference _openSound;

		public override void Wake()
		{
			base.Wake();
			_dropper.Wake();
			_animator.Randomize();
		}

		protected override void Interact()
		{
			IsOpen = true;

			Game.Audio.Play(_openSound);
			_animator.SetTrigger(OPEN_HASH);
		}

		// Animation Event
		public void DropOre()
		{
			_dropper.Drop();
			Complete();
		}

		private static readonly int OPEN_HASH = Animator.StringToHash("Open");
		private bool IsOpen { get; set; }
		protected override bool IsInteractive => false == IsOpen;
		#endregion



		#if UNITY_EDITOR
		public override void OnValidate()
		{
			base.OnValidate();
			this.TryAssign(ref _animator);
			this.TryAssign(ref _dropper);
		}
		#endif
	}
}