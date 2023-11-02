#if UNITY_EDITOR || DEVELOPMENT_BUILD || CAPTURE
#define IS_DEV
#endif

using Taijj.Input;
using DG.Tweening;
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
    /// <summary>
    /// Topmost controller for the game's flow.
    /// </summary>
    public class Game : MonoBehaviour
    {
		#region Lifecycle
		[SerializeField] private SceneModel _sceneModel;
		[SerializeField] private Catalog _catalog;
		[Space]
		[SerializeField] private UserInput _input;
		[SerializeField] private UiGame _ui;
		[SerializeField] private Updater _updater;
		[SerializeField] private GameAudio _audio;
		[Space]
		[SerializeField] private AspectRatioEnforcer _aspectEnforerPrefab;

		public void Wake()
        {
            #if IS_DEV
                IsDebugMode = true;
			#endif

			WakeTweens();

			Instance = this;
			SceneLoader = new SceneLoader(OnSceneLoaded);

			Updater.Wake();

			SceneModel.Wake(_sceneModel);
			Input.Wake();
			Ui.Wake();
			Audio.Wake();
        }

        public void SetUp()
		{
			Input.SetUp();
			Ui.SetUp(OnLoadingScreenReady);

			IsLoadingScreenReady = true;
			IsSceneLoaded = true;
			ActivateScene();
		}

		public void OnApplicationQuit() => CleanUp();
        public void CleanUp()
        {
			Input.CleanUp();
			Ui.CleanUp();

			SceneModel.CurrentRoot?.CleanUp();
        }
		#endregion



		#region Statics
		private static Game Instance { get; set; }
		public static Catalog Catalog => Instance._catalog;

		public static UserInput Input => Instance._input;
		public static UiGame Ui => Instance._ui;
		public static Updater Updater => Instance._updater;
		public static GameAudio Audio => Instance._audio;
		#endregion



		#region Scene Flow
		public static void SwitchScene(SceneData data)
		{
			Instance.SceneData = data;
			Instance.SwitchScene();
		}

		private void SwitchScene()
		{
			if (IsInSceneTransition)
				throw new Exception("PROHIBITED: Scene cannot be switched while a Scene switch is in progress!");

			IsLoadingScreenReady = false;
			IsSceneLoaded = false;

			Ui.ShowLoadingScreen(SceneData.loadingScreenKind);
			Ui.FadeScreen(new FadeData
			{
				color = Color.black,
				duration = SceneData.fadeOutDuration,
				continueOnStop = true,
				onCompleted = ExitScene
			});
		}

		private void ExitScene()
		{
			DeactivateScene();
			SceneLoader.Load(SceneData.sceneName);
		}

		private void OnLoadingScreenReady()
		{
			IsLoadingScreenReady = true;
			TryEnterScene();
		}

		private void OnSceneLoaded()
		{
			IsSceneLoaded = true;
			TryEnterScene();
		}

		private void TryEnterScene()
        {
			if (IsInSceneTransition)
				return;

			ActivateScene();
			Ui.HideLoadingScreen();
			Ui.FadeScreen(new FadeData
			{
				color = Color.black.With(0f),
				duration = SceneData.fadeInDuration,
				continueOnStop = true
			});
		}



		private void ActivateScene()
		{
			SceneRoot root = FindObjectOfType<SceneRoot>();
			SceneModel.Refresh(root);

			if (root == null)
				return;

			AspectRatioEnforcer enforcer = Instantiate(_aspectEnforerPrefab, root.transform);
			enforcer.Wake();
			enforcer.SetUp();

			root.Wake();
			root.SetUp();
		}

		private void DeactivateScene()
		{
			SceneModel.CurrentRoot.CleanUp();
			Time.timeScale = 1f;
			KillTweens();

			Input.PushMap(ActionMap.None, true);
			Ui.Skip.Deactivate();
		}



		private SceneLoader SceneLoader { get; set; }
		private SceneData SceneData { get; set; }

		private bool IsLoadingScreenReady { get; set; }
		private bool IsSceneLoaded { get; set; }
		public static bool IsInSceneTransition
		{
			get
			{
				return false == Instance.IsLoadingScreenReady
					|| false == Instance.IsSceneLoaded;
			}
		}
		#endregion



		#region State
		public static void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

		public static void ResetState()
        {
            IsDebugMode = false;
        }

        public static bool IsDebugMode { get; set; }
		#endregion



		#region Tweens
		private const string TWEEN_ID = "Game";
		private static readonly string[] TWEEN_FILTER = new string[] { TWEEN_ID };

		private void WakeTweens()
		{
			DOTween.Init(this);
			DOTween.defaultAutoKill = false;
			DOTween.defaultAutoPlay = AutoPlay.None;
		}

		public static void MakeImmortal(Tween tween)
		{
			tween.SetIsTimeScaleIgnored(true);
			tween.stringId = TWEEN_ID;
		}

		private void KillTweens()
		{
			DOTween.KillAll(false, TWEEN_FILTER);
			Ui.StopAperture();
		}
		#endregion
    }
}