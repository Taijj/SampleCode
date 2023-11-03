
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taijj.SampleCode
{
    /// <summary>
    /// Controls the logical flow for scene transitions.
    /// </summary>
    public class SceneLoader
    {
		#region Flow
		public SceneLoader(Action onReady) => OnReady = onReady;

        public void Load(string sceneName)
        {
			NextSceneName = sceneName;
            PreviousSceneName = SceneModel.Current.name;

			SceneManager.LoadSceneAsync(NextSceneName, LoadSceneMode.Additive)
				.completed += OnNextSceneLoaded;			
        }
		
		private void OnNextSceneLoaded(AsyncOperation op)
		{
			op.completed -= OnNextSceneLoaded;			
            SceneManager.UnloadSceneAsync(PreviousSceneName)
                .completed += OnPreviousSceneUnloaded;
		}

        private void OnPreviousSceneUnloaded(AsyncOperation op)
        {
            op.completed -= OnPreviousSceneUnloaded;
            
			Resources.UnloadUnusedAssets();
			OnReady();
        }        



        private string PreviousSceneName { get; set; }
        private string NextSceneName { get; set; }
		public Action OnReady { get; private set; }
        #endregion
    }
}