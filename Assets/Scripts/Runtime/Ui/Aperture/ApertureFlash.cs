
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	[Serializable]
	public class FlashData
	{
		public Color color = Color.white;
		public float inDuration = 0.2f;
		public float outDuration = 0.5f;
		public int times = 1;
		public Action onCompleted;
	}

	/// <summary>
	/// <see cref="UiAperture"/> helper for screen flasing.
	/// </summary>
	public class ApertureFlash
    {
		#region LifeCycle
		public ApertureFlash(Image faceplate)
    	{
			Plate = faceplate;
			Tween = Plate.DOColor(Color.white, 0f);
			Game.MakeImmortal(Tween);
		}

		public void CleanUp() => Tween.Kill();

		private Image Plate { get; set; }
		private Tween Tween { get; set; }
		#endregion



		#region Main
		public void Do(FlashData data)
		{
			Tween.Pause();

			float totalDuration = data.inDuration + data.outDuration;
			if (totalDuration <= 0f || data.times <= 0)
			{
				Plate.color = data.color;
				data.onCompleted?.Invoke();
				return;
			}

			Data = data;
			Counter = 0;
			FlashIn();
		}

		private void FlashIn()
		{
			Plate.color = Data.color.With(0f);
			Tween.SetValues(Plate.color, Data.color, Data.inDuration)
				.SetEase(Ease.OutSine)
				.OnComplete(FlashOut);
		}

		private void FlashOut()
		{
			Tween.SetValues(Plate.color, Data.color.With(0f), Data.outDuration)
				.SetEase(Ease.InSine)
				.OnComplete(Complete);
		}

		private void Complete()
		{
			Counter++;
			if (Counter < Data.times)
				FlashIn();
			else
				Data.onCompleted?.Invoke();
		}

		public void Stop() => Tween.Pause();

		private FlashData Data { get; set; }
		private int Counter { get; set; }
		#endregion

	}
}