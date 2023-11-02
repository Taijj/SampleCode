using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Interactor to Trigger Dialog with the given DialogLines.
	/// </summary>
	public class DialogTrigger : Interactor
	{
		#region LifeCycle
		[Space, Header("Cutscene")]
		[SerializeField] private bool _endsWithCutscene;
		[SerializeField] private float _fadeOutDelay;
		[SerializeField] private float _fadeOutDuration;

		[Space, Header("Speaker Setup")]
		[SerializeField] private UiMap _uiMap;
		[SerializeField] private SpeakerData _leftSpeaker;
		[SerializeField] private SpeakerData _rightSpeaker;

		[Space]
		[SerializeField] private DialogLine[] _dialogLines;
		[SerializeField] private EventReference _music;



		public override void Wake()
		{
			base.Wake();

			CanStartDialog = true;
		}

		public override void CleanUp()
		{
			base.CleanUp();
			_uiMap.OnSubmit -= OnSubmit;
			Level.UiLevel.OnCancel -= CompleteDialog;
		}
		#endregion



		#region Interaction
		private UiDialog _uiDialog;

		protected override void Enter()
		{
			base.Enter();

			Level.Audio.StopMusic();
			Game.Audio.Play(_music);

			_uiMap.OnSubmit += OnSubmit;

			_uiDialog = Level.UiLevel.OpenDialog(_leftSpeaker, _rightSpeaker);
			_uiDialog.OnDialogComplete += CompleteDialog;
			Level.UiLevel.OnCancel += CancelDialog;

			_uiDialog.ShowDialog(_dialogLines);
		}

		private void OnSubmit() => _uiDialog.ContinueDialog();

		private void CompleteDialog()
		{			
			Level.UiLevel.ReturnToLastLayer();
			CancelDialog();
		}		

		private void CancelDialog()
		{
			_uiMap.OnSubmit -= OnSubmit;
			_uiDialog.OnDialogComplete -= CompleteDialog;
			Level.UiLevel.OnCancel -= CancelDialog;

			CanStartDialog = false;

			Game.Audio.Stop(_music);
			if (_endsWithCutscene)
				StartCutScene();
			else
				Level.Audio.PlayMusic();
		}

		private bool CanStartDialog { get; set; }
		protected override bool IsInteractive => CanStartDialog;
		#endregion



		#region Cutscene
		private void StartCutScene()
		{
			Level.Hero.TransitTo(typeof(HeroCutscene));
			new DelayedCall(BeginFade, _fadeOutDelay).Start();
		}

		private void BeginFade()
		{			
			Game.Ui.FadeScreen(new FadeData
			{
				color = Color.black,
				duration = _fadeOutDuration,
				onCompleted = OnFadedOut
			});
		}

		private void OnFadedOut()
		{
			Game.SwitchScene(new SceneData { sceneName = SceneModel.GetSubsequentSceneName() });
		}
		#endregion
	}
}
