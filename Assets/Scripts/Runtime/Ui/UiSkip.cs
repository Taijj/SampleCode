using DG.Tweening;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Taijj.SampleCode
{
	/// <summary>
	/// UI showing text and progressbar for skip interactions
	/// </summary>
	public class UiSkip : MonoBehaviour
	{
		#region Initialization
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private Image _skipProgressbar;
		[SerializeField, Range(0.05f, 1f)] private float _fadeDuration = 0.2f;
		[Space]
		[SerializeField] private PresentationMap _input;
		[SerializeField] private EventReference _holdSound;
		[SerializeField] private EventReference _completeSound;

		public void Wake()
		{
			_group.alpha = 0f;
			gameObject.SetActive(false);

			Delay = DOVirtual.DelayedCall(1f, OnDelayCompleted);
			Appear = _group.DOFade(1f, _fadeDuration);
			Disappear =_group.DOFade(0f, _fadeDuration);

			Game.MakeImmortal(Delay);
			Game.MakeImmortal(Appear);
			Game.MakeImmortal(Disappear);
		}

		public void CleanUp()
		{
			Delay.Kill();
			Appear.Kill();
			Disappear.Kill();

			RemoveSkipListeners();
			RemoveFadelisteners();
		}

		private Tween Delay { get; set; }
		private Tween Appear { get; set; }
		private Tween Disappear { get; set; }
		#endregion



		#region Main
		/// <summary>
		/// Activates "Hold to Skip".
		/// </summary>
		/// <param name="isNextAny">If true, the "Next" input action, is treated as an "Any" input action as well.</param>
		public void Activate(bool isNextAny = true)
		{
			gameObject.SetActive(true);
			_group.alpha = 0f;
			IsNextAny = isNextAny;

			Delay.Rewind();
			Delay.Play();
		}

		private void OnDelayCompleted()
		{
			UpdateSkipProgress(0f);

			AddSkipListeners();
			AddFadeListeners();
			Game.Input.PushMap(_input);
		}

		public void Deactivate()
		{
			Game.Input.PopMap();
			RemoveSkipListeners();
			RemoveFadelisteners();

			Delay.Pause();
			FadeOut();
		}
		#endregion



		#region Skipping
		private void AddSkipListeners()
		{
			_input.UpdateSkipProgress += UpdateSkipProgress;
			_input.OnSkipCompleted += OnSkipCompleted;
			_input.OnSkipStarted += StartSkipping;
			_input.OnSkipCanceled += CancelSkipping;
		}

		private void RemoveSkipListeners()
		{
			_input.UpdateSkipProgress -= UpdateSkipProgress;
			_input.OnSkipCompleted -= OnSkipCompleted;
			_input.OnSkipStarted -= StartSkipping;
			_input.OnSkipCanceled -= CancelSkipping;
		}

		private void StartSkipping()
		{
			Game.Audio.Play(_holdSound);
			FadeIn();
		}

		private void CancelSkipping()
		{
			Game.Audio.Stop(_holdSound);
			FadeOut();
		}

		private void UpdateSkipProgress(float fillAmount) => _skipProgressbar.fillAmount = fillAmount;

		private void OnSkipCompleted()
		{
			Game.Audio.Stop(_holdSound);
			Game.Audio.Play(_completeSound);

			FadeOut();
			OnSkipped?.Invoke();
		}

		public event Action OnSkipped;
		#endregion



		#region Fading
		private void AddFadeListeners()
		{
			_input.OnAnyPress += FadeIn;
			_input.OnAnyRelease += FadeOut;

			if (false == IsNextAny)
				return;

			_input.OnNextPress += FadeIn;
			_input.OnNextRelease += FadeOut;
		}

		private void RemoveFadelisteners()
		{
			_input.OnAnyPress -= FadeIn;
			_input.OnAnyRelease -= FadeOut;

			if (false == IsNextAny)
				return;

			_input.OnNextPress -= FadeIn;
			_input.OnNextRelease -= FadeOut;
		}

		private void FadeIn()
		{
			float fraction = 1f - _group.alpha;
			Appear.SetValues(_group.alpha, 1f, _fadeDuration * fraction);
		}

		private void FadeOut()
		{
			float fraction = _group.alpha;
			Disappear.SetValues(_group.alpha, 0f, _fadeDuration * fraction);
		}

		// If true, the "Next" Input is treated as an "Any" input, too.
		private bool IsNextAny { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			_group = GetComponentInChildren<CanvasGroup>();
			_skipProgressbar = GetComponentInChildren<Image>();
		}
		#endif
	}
}