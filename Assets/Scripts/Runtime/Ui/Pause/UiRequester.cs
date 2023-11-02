using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Base for yes/no Ui Requests
	/// </summary>
	public abstract class UiRequester : UiLayer
	{
		#region LifeCycle
		[field: SerializeField] public UiButton Yes { get; private set; }
		[field: SerializeField] public UiButton No { get; private set; }

		public override void Open()
		{
			gameObject.Activate();
			Select(No);
			Deselect(Yes);
		}

		public override void Close() => gameObject.Deactivate();
		#endregion
	}
}