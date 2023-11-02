using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Taijj.HeartWarming
{
	public class TitleScreen : SceneRoot
	{
		#region LifeCycle
		[SerializeField] private UiMap _input;
		[SerializeField] private UiButton _start;
		[SerializeField] private UiButton _quit;
		[Space]
		[SerializeField] private Animator _animator;
		[SerializeField] private float _startDelay = 1f;
		[SerializeField] private float _fadeOutDuration = 1f;

		[Header("FMOD")]
		[SerializeField] private StudioBankLoader _loader;
		[SerializeField] private EventReference _music;
		[SerializeField] private EventReference _submitSound;

		public override void Wake() => _loader.Load();

		public override void SetUp()
		{
			_start.Wake();
			_quit.Wake();			
			
			Game.Audio.Play(_music);

			_delay = new DelayedCall(StartAnimation, _startDelay);
			_delay.Start();
		}

		public override void CleanUp()
		{
			_loader.Unload();
			_start.CleanUp();
			_quit.CleanUp();

			_delay.Stop();
			Game.Input.PopMap();			
			EventSystem.current.SetSelectedGameObject(null);
		}
		#endregion



		#region Main
		private static readonly int SHOW_HASH = Animator.StringToHash("Show");
		private static readonly int SHOWN_HASH = Animator.StringToHash("Shown");

		private DelayedCall _delay;

		private void StartAnimation()
		{
			_animator.TriggerInstantly(SHOW_HASH, out float duration);			
			_delay = new DelayedCall(FinishAnimation, duration);
			_delay.Start();
		}

		private void FinishAnimation()
		{
			_animator.TriggerInstantly(SHOWN_HASH, out float _);

			Game.Input.PushMap(_input);
			_start.AddListener(StartGame);
			_quit.AddListener(QuitApplication);
			_start.ForceSelect();
		}

		private void StartGame()
		{
			EventSystem.current.SetSelectedGameObject(null);
			Game.Input.PopMap();
			Game.Audio.Stop(_music);
			Game.SwitchScene(new SceneData
			{
				sceneName = SceneModel.GetSubsequentSceneName(),
				fadeOutDuration = _fadeOutDuration
			});
		}

		private void QuitApplication() => Game.Quit();
		#endregion
	}
}