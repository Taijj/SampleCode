using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base for Scene entitys the hero can interact with by pressing a button,
	/// up or down.
	/// </summary>
	public class Interactor : MonoBehaviour
	{
		#region LifeCycle
		[Space, Header("Base")]
		[SerializeField] private Collider2D _collider;
		[SerializeField] private bool _isBlocking = true;
		[Space]
		[SerializeField] private GameObject _buttonKey;
		[SerializeField] private EventReference _enterSound;

		public virtual void Wake() => _buttonKey.Deactivate();

		public virtual void Respawn()
		{
			if (Hero.HasReference())
				Exit();

			// This causes a Hero exit/reenter
			_collider.enabled = false;
			_collider.enabled = IsInteractive;
			IsPerforming = false;
		}

		public virtual void CleanUp() {}

		[field: SerializeField, ReadOnly] private Hero Hero { get; set; }
		#endregion



		#region Triggering
		public void OnTriggerEnter2D(Collider2D collision) => TryEnter(collision);
		public void OnTriggerStay2D(Collider2D collision) => TryEnter(collision);
		private void TryEnter(Collider2D collider)
		{
			if (false == IsInteractive)
				return;

			if (Hero.HasReference())
				return;

			if (IsHero(collider))
				Enter();
		}

		public void OnTriggerExit2D(Collider2D collision)
		{
			if (Hero.IsNull())
				return;

			if (IsHero(collision))
				Exit();
		}

		private bool IsHero(Collider2D collider)
		{
			if (false == collider.TryGet(out Corpus corpus))
				return false;

			return corpus.gameObject.layer == Layers.HERO;
		}
		#endregion



		#region Enter, Exit
		protected virtual void Enter()
		{
			Hero = Level.Hero;
			Hero.Interactor = this;

			_buttonKey.Activate();
			Game.Audio.Play(_enterSound);
		}

		protected virtual void Exit()
		{
			Hero.Interactor = null;
			Hero = null;

			_buttonKey.Deactivate();
		}
		#endregion



		#region Interacting
		public void Perform()
		{
			if (IsPerforming)
				return;

			if(_isBlocking)
			{
				Game.Input.PushMap(null);
				Level.Horde.IsPaused = true;
			}
			IsPerforming = true;

			_collider.enabled = false;
			Interact();
		}

		protected virtual void Interact() => Complete();
		protected virtual void Complete()
		{
			IsPerforming = false;
			_collider.enabled = true;

			if(_isBlocking)
			{
				Level.Horde.IsPaused = false;
				Game.Input.PopMap();
			}
		}

		protected virtual bool IsInteractive => true;
		private bool IsPerforming { get; set; }
		#endregion



		#if UNITY_EDITOR
		public virtual void OnValidate()
		{
			this.TryAssign(ref _collider);
		}
		#endif
	}
}