using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// This game's "AudioManager", manages and handles triggering of Audio events.
	/// </summary>
    public class GameAudio : MonoBehaviour
    {
		#region Main
		private const int PLAYING_INSTANCES_CAPACITY = 50;		

		public void Wake()
		{
			PlayingInstances = new List<EventInstance>(PLAYING_INSTANCES_CAPACITY);
		}

		public bool TryCreate(EventReference e, Transform target, out EventInstance instance)
		{
			instance = RuntimeManager.CreateInstance(e);
			if (false == instance.isValid())
				return false;

			if (target.HasReference())
				RuntimeManager.AttachInstanceToGameObject(instance, target);

			return true;
		}		

		private void CleanPlayingInstances()
		{
			for (int i = PlayingInstances.Count-1; i >= 0; i--)
			{
				PlayingInstances[i].getPlaybackState(out PLAYBACK_STATE state);
				if (state == PLAYBACK_STATE.STOPPED)
					PlayingInstances.RemoveAt(i);
			}
		}

		private List<EventInstance> PlayingInstances { get; set; }
		#endregion



		#region LifeCycle
		public void Play(EventReference e, Transform target = null)
		{
			if (e.IsNull)
				return;

			if (IsCollapse(e.Guid))
				return;

			#if UNITY_EDITOR
				TryLogStart(e);
			#endif

			if (false == TryCreate(e, target, out EventInstance instance))
				return;

			instance.start();
			instance.release();

			CleanPlayingInstances();
			PlayingInstances.Add(instance);
		}			
		
		private bool IsCollapse(FMOD.GUID guid)
		{
			if (PlayingInstances.Count == 0)
				return false;

			float time = Time.time;
			bool IsSameTime = LastEventTime == time;
			LastEventTime = time;

			if (false == IsSameTime)
				return false;								

			PlayingInstances.Last().getDescription(out EventDescription desc);
			desc.getID(out FMOD.GUID id);
			return id == guid;
		}

		public void Stop(EventReference e)
		{
			if (e.IsNull)
				return;			

			CleanPlayingInstances();
			for (int i = PlayingInstances.Count-1; i >= 0; i--)
			{
				EventInstance instance = PlayingInstances[i];
				instance.getDescription(out EventDescription desc);
				desc.getID(out FMOD.GUID id);
				if (id != e.Guid)
					continue;

				#if UNITY_EDITOR
					TryLogStop(e);
				#endif

				instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				PlayingInstances.Remove(instance);
			}
		}		

		public void SetParameterOf(EventReference e, string parameter, float value)
		{
			for(int i = 0; i < PlayingInstances.Count; i++)
			{
				EventInstance instance = PlayingInstances[i];
				instance.getDescription(out EventDescription desc);
				desc.getID(out FMOD.GUID id);
				if (id != e.Guid)
					continue;

				instance.setParameterByName(parameter, value);
				return;
			}
		}

		private float LastEventTime { get; set; }
		#endregion
			


		#region Music Volume
		private const string MUSIC_VOLUME_PARAMETER = "MusicVolume";

		public void ToggleMusic()
		{
			CurrentMusicVolume = CurrentMusicVolume == 1 ? 0 : 1;
			RuntimeManager.StudioSystem.setParameterByName(MUSIC_VOLUME_PARAMETER, CurrentMusicVolume);
		}
		[field: SerializeField, ReadOnly] private int CurrentMusicVolume { get; set; }
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Debug")]
		[SerializeField] private bool _logEvents;
		[SerializeField] private string _eventLogFilter;
		[SerializeField] private EventReference _debugEvent;

		[Button(nameof(PlayDebugEvent))] public bool playTarget;
		public void PlayDebugEvent() => Play(_debugEvent);

		[Button(nameof(StopDebugEvent))] public bool stopTarget;
		public void StopDebugEvent() => Stop(_debugEvent);

		private void TryLogStart(EventReference e)
		{
			if (false == _logEvents)
				return;

			string message = $"Start: {e.Path}";
			if (string.IsNullOrEmpty(_eventLogFilter))
			{
				Note.Log(message, ColorAddons.Lime);
				return;
			}

			if (e.Path.Contains(_eventLogFilter))
				Note.Log(message, ColorAddons.Lime);
		}

		private void TryLogStop(EventReference e)
		{
			if (false == _logEvents)
				return;

			string message = $"Stop: {e.Path}";
			if (string.IsNullOrEmpty(_eventLogFilter))
			{
				Note.Log(message, ColorAddons.Orange);
				return;
			}

			if(e.Path.Contains(_eventLogFilter))
				Note.Log(message, ColorAddons.Orange);
		}
		#endif
	}
}