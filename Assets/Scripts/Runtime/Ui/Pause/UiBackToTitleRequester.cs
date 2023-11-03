using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// UiRequester that returns to Title on Yes and goes back one UiLayer on No
	/// </summary>
    public class UiBackToTitleRequester : UiRequester
    {
		#region LifeCycle
		[SerializeField] private SceneData _titleScene;

		public void Wake()
		{
			Yes.Wake();
			No.Wake();
			Yes.AddListener(BackToTitle);
			No.AddListener(BackToPause);
		}

		public void CleanUp()
		{
			Yes.RemoveListener(BackToTitle);
			No.RemoveListener(BackToPause);
		}
		#endregion



		#region Ui Events
		private void BackToPause() => Level.UiLevel.ReturnToLastLayer();

		private void BackToTitle()
		{
			Level.Hero.godMode = true;
			Level.UiLevel.ChangeScene(_titleScene);
		}
		#endregion
	}
}
