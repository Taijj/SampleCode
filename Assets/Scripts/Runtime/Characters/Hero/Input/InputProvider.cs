using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base for providing inputs to the <see cref="Hero"/>
	/// </summary>
	public abstract class InputProvider : MonoBehaviour
	{
		#region Relays
		public Vector2 Move { get; protected set; }
		public Action OnJumpPress { protected get; set; }
		public Action OnJumpRelease { protected get; set; }

		public Action OnShootFire { protected get; set; }
		public Action OnShootIce { protected get; set; }

		public Action OnSpecial { protected get; set; }
		public static Action<SpecialKind> OnSelectSpecial { get; set; }

		public bool IsBreathingFire { get; protected set; }
		public bool IsBreathingIce { get; protected set; }
		public Vector2 BreathDirection { get; protected set; }
		#endregion



		#region Updates
		public virtual void Enable() { }
		public virtual void Disable() { }
		public abstract void OnUpdate();
		#endregion



		#region Misc
		private Interactor _interactor;
		private bool _hasInteractor;

		public bool TryGetInteractor(out Interactor interactor)
		{
			interactor = _interactor;
			_hasInteractor = _interactor.HasReference();
			return _hasInteractor;
		}

		public void Set(Interactor interactor) => _interactor = interactor;



		public bool IsUpHeld { get; protected set; }
		public bool IsDownHeld { get; protected set; }
		#endregion
	}
}