using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// For simple Scenes that just show some text and will load a next
	/// scene, when a button is pressed.
	/// </summary>
	public class StoryScreen : SceneRoot
	{
		[SerializeField] private UiMap _input;
		[SerializeField] private SceneField _sceneToLoad;		
		[SerializeField] private float _fadeOutDuration = 1f;
		[SerializeField] private LoadingScreenKind _loadingScreenKind;
		[Space, Header("FMOD")]
		[SerializeField] private StudioBankLoader _loader;
		[SerializeField] private EventReference _music;
		[SerializeField] private EventReference _submitSound;

		public override void Wake()
		{
			_input.OnSubmit += OnSubmit;
			_loader.Load();
		}

		public override void SetUp()
		{
			Game.Input.PushMap(_input);
			Game.Audio.Play(_music);
		}

		public override void CleanUp()
		{
			_input.OnSubmit -= OnSubmit;
			_loader.Unload();
		}

		private void OnSubmit()
		{
			_input.OnSubmit -= OnSubmit;
			Game.Audio.Play(_submitSound);
			Game.Audio.Stop(_music);

			Game.SwitchScene(new SceneData
			{
				sceneName = _sceneToLoad.SceneName,
				loadingScreenKind = _loadingScreenKind,
				fadeOutDuration = _fadeOutDuration
			});
		}
	}
}
