using System;
using UnityEngine;

namespace Taijj.SampleCode
{
    public class UiGame : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private UiLoadingScreen[] _loadingScreens;
		[SerializeField] private UiAperture _aperture;
		[SerializeField] private UiSkip _skip;

		public void Wake()
		{
			for (int i = 0; i < _loadingScreens.Length; i++)
				_loadingScreens[i].Wake();

			_aperture.Wake();
			_skip.Wake();
		}

		public void SetUp(Action onLoadingScreenReady)
		{
			for (int i = 0; i < _loadingScreens.Length; i++)
				_loadingScreens[i].SetUp(onLoadingScreenReady);
		}

		public void CleanUp()
		{
			for (int i = 0; i < _loadingScreens.Length; i++)
				_loadingScreens[i].CleanUp();

			_aperture.CleanUp();
			_skip.CleanUp();
		}

		public UiSkip Skip => _skip;
		#endregion



		#region Loading Screen
		public void ShowLoadingScreen(LoadingScreenKind kind)
		{
			for (int i = 0; i < _loadingScreens.Length; i++)
			{
				CurrentLoadingScreen = _loadingScreens[i];
				if (CurrentLoadingScreen.Kind == kind)
					break;
			}
			CurrentLoadingScreen.Show();
		}

		public void HideLoadingScreen()
		{
			if (CurrentLoadingScreen.IsNull())
				return;

			CurrentLoadingScreen.Hide();
			CurrentLoadingScreen = null;
		}

		public UiLoadingScreen CurrentLoadingScreen { get; set; }
		#endregion



		#region Aperture
		public void FadeScreen(FadeData data) => _aperture.Fade.Do(data);
		public void FlashScreen(FlashData data) => _aperture.Flash.Do(data);
		public void StopAperture() => _aperture.Stop();
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if(_loadingScreens.IsFaultyFixed()) _loadingScreens = GetComponentsInChildren<UiLoadingScreen>();
			if(_aperture.IsNull()) _aperture = GetComponentInChildren<UiAperture>();
			if(_skip.IsNull()) _skip = GetComponentInChildren<UiSkip>();
		}
		#endif
	}
}