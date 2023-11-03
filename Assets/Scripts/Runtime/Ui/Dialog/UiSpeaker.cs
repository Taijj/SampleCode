using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Taijj.SampleCode
{
    public class UiSpeaker : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private Image _portrait;
		[SerializeField] private Image _textBox;
		[SerializeField] private Image _label;
		[SerializeField] private TMP_Text _nameTextMesh;

		public void Wake(UiDialog.SpeakerTween config)
		{
			ActivationTween = DOTween.To(GetPortraitScale, SetPortraitScale, config.activeScale, config.duration)
				.SetEase(config.ease);
			DeactivationTween = DOTween.To(GetPortraitScale, SetPortraitScale, config.inactiveScale, config.duration)
				.SetEase(config.ease);

			ActiveTint = config.activeTint;
			InactiveTint = config.inactiveTint;
		}

		public void CleanUp()
		{
			ActivationTween?.Kill();
			DeactivationTween?.Kill();
		}

		public void SetPortraitScale(float value) => _portrait.rectTransform.localScale = Vector3.one * value;
		public float GetPortraitScale() => _portrait.rectTransform.localScale.x;

		private Tween ActivationTween { get; set; }
		private Tween DeactivationTween { get; set; }
		#endregion



		#region Data
		public void Feed(SpeakerData data, Sprite overridePortrait = null)
		{
			Character = data.character;
			_nameTextMesh.text = Character.ToString();

			_portrait.sprite = overridePortrait.IsNull() ? data.portrait : overridePortrait;
			_textBox.sprite = data.textBoxBg;
			_label.sprite = data.labelBg;
		}		

		public Character Character { get; private set; }
		#endregion



		#region Activate/Deactivate
		public void Activate()
		{
			_textBox.Activate();
			_label.Activate();

			_portrait.color = ActiveTint;
			ActivationTween.Restart();
		}

		public void Deactivate()
		{
			_textBox.Deactivate();
			_label.Deactivate();

			_portrait.color = InactiveTint;
			DeactivationTween.Restart();
		}

		private Color ActiveTint { get; set; }
		private Color InactiveTint { get; set; }
		#endregion
	}
}