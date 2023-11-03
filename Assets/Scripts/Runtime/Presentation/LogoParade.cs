using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Taijj.SampleCode
{
    public class LogoParade : SceneRoot
    {
        #region LifeCycle
        [SerializeField] private Image[] _logos;
		[SerializeField] private int _unskipableCount;
        [Space]
        [SerializeField] private float _initialDelay;
        [SerializeField] private float _fadeInDuration = 1f;
        [SerializeField] private float _stayDuration = 1f;
        [SerializeField] private float _fadeOutDuration = 1f;

        public override void Wake()
        {
            foreach (Image image in _logos)
                image.color = image.color.With(0f);
        }

        public override void SetUp() => BeginParade();

        public override void CleanUp() => Tween?.Kill();
		#endregion



		#region Flow
		private void BeginParade()
		{
			Index = -1;
			Tween = DOVirtual.DelayedCall(_initialDelay, StartNext)
				.Play();
        }

        private void StartNext()
        {
            Index++;
			if(Index == _unskipableCount-1)
			{
				Game.Ui.Skip.Activate();
				Game.Ui.Skip.OnSkipped += Complete;
			}

            Tween = CurrentImage.DOColor(CurrentImage.color.With(1f), _fadeInDuration)
                .OnComplete(OnFadeInCompleted)
				.Play();

        }
        private void OnFadeInCompleted()
        {
            Tween = CurrentImage.DOColor(CurrentImage.color.With(0f), _fadeOutDuration)
                .SetDelay(_stayDuration)
                .OnComplete(OnLogoCompleted)
				.Play();
        }

        private void OnLogoCompleted()
        {
            if (Index < _logos.Length - 1)
                StartNext();
            else
                Complete();
        }

        private void Complete()
        {
			Tween.Kill();

			Game.Ui.Skip.OnSkipped -= Complete;
			Game.Ui.Skip.Deactivate();
			Game.SwitchScene(new SceneData
			{
				sceneName = SceneModel.GetSubsequentSceneName()
			});
        }



        private Tween Tween { get; set; }
        private int Index { get; set; }
        private Image CurrentImage => _logos[Index];
        #endregion



        #if UNITY_EDITOR
        public void OnValidate()
        {
            if(_logos == null || _logos.Length == 0)
                _logos = GetComponentsInChildren<Image>();
        }
        #endif
    }
}