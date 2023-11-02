using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Ingame Pause Ui
	/// </summary>
	public class UiPause : UiLayer
    {
		#region LifeCycle
		[SerializeField] private UiButton _continue;
		[SerializeField] private UiButton _backToTitle;



		public void Wake()
		{
			_continue.Wake();
			_backToTitle.Wake();
			_continue.AddListener(Continue);
			_backToTitle.AddListener(BackToTitle);
		}

		public void CleanUp()
		{
			_continue.RemoveListener(Continue);
			_backToTitle.RemoveListener(BackToTitle);
		}
		#endregion



		#region Pause Logic
		public override void Open()
		{
			gameObject.Activate();
			Deselect(_backToTitle);
			Select(_continue);

			Time.timeScale = 0f;
		}

		public override void Close()
		{
			Time.timeScale = 1f;
			gameObject.Deactivate();
		}

		private void Continue() => Level.UiLevel.ReturnToLastLayer();

		private void BackToTitle() => Level.UiLevel.OpenRequester();
		#endregion
	}
}
