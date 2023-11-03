using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// TopLevel Controller for all Uis that can appeaar in a Level
	/// Controlls the opening and closing of UI Layers as well as the Selection Cursor
	/// </summary>
	public class UiLevel : MonoBehaviour
	{
		#region LifeCycle
		[Header("Ui Layer")]
		[SerializeField] private UiPause _uiPause;
		[SerializeField] private UiBackToTitleRequester _uiBackToTitleRequester;
		[SerializeField] private UiDialog _uiDialog;

		[Header("Input")]
		[SerializeField] private HeroMap _heroMap;
		[SerializeField] private UiMap _uiMap;
		[SerializeField] private EventReference _cancelSound;
		[Space]
		[SerializeField] private GameObject _watermark;

		private List<UiLayer> _activeLayers = new List<UiLayer>();



		public void Wake()
		{
			_heroMap.OnPause += OpenPause;
			_heroMap.OnCheat += ToggleWatermark;
			_uiMap.OnCancel += Cancel;
			_uiMap.OnClose += Cancel;

			_uiPause.Wake();
			_uiBackToTitleRequester.Wake();

			_uiDialog.Wake();
		}

		public void CleanUp()
		{
			_heroMap.OnPause -= OpenPause;
			_heroMap.OnCheat -= ToggleWatermark;
			_uiMap.OnCancel -= Cancel;
			_uiMap.OnClose -= Cancel;

			_uiPause.CleanUp();
			_uiBackToTitleRequester.CleanUp();

			_uiDialog.CleanUp();
		}

		private void ToggleWatermark(DevCheat cheat)
		{
			if (cheat == DevCheat.ToggleWatermark)
				_watermark.SetActive(!_watermark.activeSelf);
		}
		#endregion



		#region Ui Management
		private void OpenPause()
		{
			_uiPause.Open();
			AddToActiveLayers(_uiPause);
		}

		public void OpenRequester()
		{
			_uiBackToTitleRequester.Open();
			AddToActiveLayers(_uiBackToTitleRequester);
		}

		public UiDialog OpenDialog(SpeakerData leftSpeaker, SpeakerData rightSpeaker)
		{
			_uiDialog.Open(leftSpeaker, rightSpeaker);
			AddToActiveLayers(_uiDialog);

			return _uiDialog;
		}

		private void Cancel()
		{
			if (_activeLayers.Count <= 0)
				return;

			Game.Audio.Play(_cancelSound);
			ReturnToLastLayer();
			OnCancel?.Invoke();
		}

		public void ChangeScene(SceneData sceneData)
		{
			for (int i = 0; i < _activeLayers.Count; i++)
				_activeLayers[i].Close();

			_activeLayers.Clear();

			Game.Input.PopMap();
			Game.Input.PopMap(); // Also pops Hero Input
			Game.SwitchScene(sceneData);
		}

		private void AddToActiveLayers(UiLayer uiLayer)
		{
			_activeLayers.Add(uiLayer);

			if (_activeLayers.Count == 1)
				Game.Input.PushMap(_uiMap);
		}

		public void ReturnToLastLayer()
		{
			_activeLayers[_activeLayers.Count - 1].Close();
			_activeLayers.Remove(_activeLayers[_activeLayers.Count - 1]);

			if (_activeLayers.Count > 0)
				_activeLayers[_activeLayers.Count - 1].ActivateDefault();

			if (_activeLayers.Count <= 0)
				Game.Input.PopMap();
		}

		public event Action OnCancel;
		#endregion



		#if UNITY_EDITOR
		private void OnValidate()
		{
			_uiPause = GetComponentInChildren<UiPause>(true);
			_uiBackToTitleRequester = GetComponentInChildren<UiBackToTitleRequester>(true);
		}
		#endif
	}
}
