
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	[Serializable]
	public class FadeData
	{
		public Color color;
		public float duration;
		public Action onCompleted;

		public bool useAdditiveFaceplate;
		public bool ignoreTimeScale;
		public bool continueOnStop;
	}

	/// <summary>
	/// <see cref="UiAperture"/> helper for screen fading.
	/// </summary>
	public class ApertureFade
    {
		#region LifeCycle
		public ApertureFade(Image defaultFaceplate, Image additiveFaceplate)
		{
			PlateDefault = defaultFaceplate;
			PlateAdditive = additiveFaceplate;

			TweenDefault = PlateDefault.DOColor(Color.white, 0f)
				.OnComplete(Complete);
			Game.MakeImmortal(TweenDefault);

			TweenAdditive = PlateAdditive.DOColor(Color.white, 0f)
				.OnComplete(Complete);
			Game.MakeImmortal(TweenAdditive);
		}

		public void CleanUp()
		{
			TweenAdditive?.Kill();
			TweenDefault?.Kill();
		}

		private Image PlateDefault { get; set; }
		private Image PlateAdditive { get; set; }

		private Tween TweenDefault { get; set; }
		private Tween TweenAdditive { get; set; }
		#endregion



		#region Main
		public void Do(FadeData data)
		{
			Data = data;
			if (Data.useAdditiveFaceplate)
				Tween(PlateAdditive, TweenAdditive);
			else
				Tween(PlateDefault, TweenDefault);
		}

		private void Tween(Image plate, Tween tween)
		{
			if (Data.duration <= 0f)
			{
				plate.color = Data.color;
				Complete();
				return;
			}

			bool fadesIn = plate.color.a > Data.color.a;
			Ease ease = fadesIn ? Ease.InQuad : Ease.OutQuad;

			tween.SetValues(plate.color, Data.color, Data.duration)
				.SetEase(ease)
				.SetIsTimeScaleIgnored(Data.ignoreTimeScale);
		}

		private void Complete() => Data.onCompleted?.Invoke();

		public void Stop()
		{
			if (Data == null)
				return;

			if (Data.continueOnStop)
				return;

			TweenDefault.Pause();
			TweenAdditive.Pause();
		}

		private FadeData Data { get; set; }
		#endregion
	}
}