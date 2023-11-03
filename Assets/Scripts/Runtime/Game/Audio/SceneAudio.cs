using UnityEngine;
using FMODUnity;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Takes care of Scene specific Audio.
	/// </summary>
	[RequireComponent(typeof(StudioBankLoader))]
	public class SceneAudio : MonoBehaviour
	{
		#region Main
		[SerializeField] private StudioBankLoader _loader;
		[SerializeField] private EventReference _music;
		[Space]
		[SerializeField] private GameObject[] _relatedObjects;

		public void Wake()
		{
			_loader.Load();
			for (int i = 0; i < _relatedObjects.Length; i++)
				_relatedObjects[i].Activate();
		}
		public void CleanUp()
		{
			Game.Audio.Stop(_music);
			_loader.Unload();
		}



		public void PlayMusic() => Game.Audio.Play(_music);
		public void StopMusic() => Game.Audio.Stop(_music);
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _loader);

			if (_relatedObjects.IsFaulty())
				return;

			for (int i = 0; i < _relatedObjects.Length; i++)
				_relatedObjects[i].Deactivate();
		}
		#endif
	}
}