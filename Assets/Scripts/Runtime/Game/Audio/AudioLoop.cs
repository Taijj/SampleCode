
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
    public class AudioLoop
    {
		#region LifeCycle
		public AudioLoop(EventReference e, Transform target)
    	{
			Target = target;

			Game.Audio.TryCreate(e, target, out EventInstance instance);
			Instance = instance;
    	}
		
		public void Destroy()
		{
			Instance.release();
			Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);			
		}

		private Transform Target { get; set; }
		private EventInstance Instance { get; set; }
		#endregion



		#region Main
		public void Play()
		{
			if (IsPlaying)
				return;
						
			IsPlaying = true;
			RuntimeManager.AttachInstanceToGameObject(Instance, Target);
			Instance.start();
		}
		
		public void OnUpdate(string parameter, float value)
		{
			if (false == IsPlaying)
				return;
						
			Instance.setParameterByName(parameter, value);		
		}
		
		public void Stop()
		{
			if (false == IsPlaying)
				return;
						
			IsPlaying = false;			
			Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}

		private bool IsPlaying { get; set; }
		#endregion
	}
}