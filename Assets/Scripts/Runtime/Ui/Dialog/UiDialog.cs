using DG.Tweening;
using FMODUnity;
using System;
using TMPro;
using UnityEngine;

namespace Taijj.HeartWarming
{
	public class UiDialog : UiLayer
	{
		#region LifeCycle
		[Serializable]
		public struct SpeakerTween
		{
			public Color activeTint;
			public Color inactiveTint;
			public float activeScale;
			public float inactiveScale;
			public float duration;
			public Ease ease;
		}

		[Space, Header("Dialog")]
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private float _fadeduration;
		[SerializeField] private TextMeshProUGUI _dialogText;
		[SerializeField] private float _typingDelay;
		[Space, Header("Speakers")]
		[SerializeField] private UiSpeaker _leftSpeaker;
		[SerializeField] private UiSpeaker _rightSpeaker;
		[SerializeField] private SpeakerTween _speakerTween;
		[Space, Header("Audio")]
		[SerializeField] private EventReference _nextSound;
		[SerializeField] private EventReference _completeSound;

		private DialogLine[] _dialogLines;
		private int _dialogLineIndex = 0;



		public void Wake()
		{
			_leftSpeaker.Wake(_speakerTween);
			_rightSpeaker.Wake(_speakerTween);
			gameObject.Deactivate();

			FadeToVisible = DOTween.To(SetCanvasGroupalpha,0,1,_fadeduration);
			FadeToInvisible = DOTween.To(SetCanvasGroupalpha,1,0,_fadeduration).OnComplete(CompleteClose);
		}

		public override void Open() {}
		public void Open(SpeakerData leftData, SpeakerData rightData)
		{
			_leftSpeaker.Feed(leftData);
			_rightSpeaker.Feed(rightData);

			_canvasGroup.alpha = 0f;
			gameObject.Activate();

			FadeToVisible.Restart();
		}

		public override void Close()
		{
			FadeToInvisible.Restart();
		}

		private void CompleteClose()
		{
			gameObject.Deactivate();
		}

		public void CleanUp()
		{
			_leftSpeaker.CleanUp();
			_rightSpeaker.CleanUp();

			FadeToVisible?.Kill();
			FadeToInvisible?.Kill();
		}
		#endregion



		#region Dialog
		public void ShowDialog(DialogLine[] dialogLines)
		{
			_dialogLines = dialogLines;
			_dialogLineIndex = 0;

			IsFirstDialogLine = true;
			ContinueDialog();
		}

		public void ContinueDialog()
		{
			if (_dialogLineIndex >= _dialogLines.Length)
			{
				Game.Audio.Play(_completeSound);
				OnDialogComplete.Invoke();
				return;
			}

			if(false == IsFirstDialogLine)
				Game.Audio.Play(_nextSound);
			IsFirstDialogLine = false;

			if (TextTween.IsActive())
			{
				TextTween.Complete();
				return;
			}

			DialogLine dialogLine = _dialogLines[_dialogLineIndex];

			SetUpDialogVisuals(dialogLine);

			StartTextTween();
		}

		private void StartTextTween()
		{
			float duration = _dialogText.text.Length * _typingDelay;
			SetDialogLineVisibleCharacters(0);
			TextTween = DOTween.To(GetDialogLineVisibleCharacters, SetDialogLineVisibleCharacters, _dialogText.text.Length, duration)
				.SetEase(Ease.Linear)
				.OnComplete(OnComplete)
				.Play();
		}

		private void SetUpDialogVisuals(DialogLine dialogLine)
		{
			if (dialogLine.speakerData.character == _leftSpeaker.Character)
			{
				_leftSpeaker.Feed(dialogLine.speakerData, dialogLine.speakerPortraitOverride);
				_leftSpeaker.Activate();

				_rightSpeaker.Deactivate();
			}
			else
			{
				_rightSpeaker.Feed(dialogLine.speakerData, dialogLine.speakerPortraitOverride);
				_rightSpeaker.Activate();

				_leftSpeaker.Deactivate();
			}

			_dialogText.text = dialogLine.dialogLine;
		}
		
		private void OnComplete()
		{
			TextTween.Kill();

			_dialogLineIndex += 1;
		}

		public event Action OnDialogComplete;
		private bool IsFirstDialogLine { get; set; }



		public void SetDialogLineVisibleCharacters(float value) => _dialogText.maxVisibleCharacters = (int)value;
		public float GetDialogLineVisibleCharacters() => _dialogText.maxVisibleCharacters;
		public void SetCanvasGroupalpha(float value) => _canvasGroup.alpha = value;


		private Tween TextTween { get; set; }
		private Tween FadeToVisible { get; set; }
		private Tween FadeToInvisible { get; set; }
		#endregion
	}
}
