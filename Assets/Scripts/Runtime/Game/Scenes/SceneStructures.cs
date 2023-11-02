using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
    [Serializable]
    public class SceneData
    {
        public string sceneName;

        public float fadeOutDuration = 1f;
        public float fadeInDuration = 1f;
        public LoadingScreenKind loadingScreenKind;
    }

	/// <summary>
	/// Top level controller of any given Scene.
	/// </summary>
    public abstract class SceneRoot : MonoBehaviour
    {
        public abstract void Wake();
        public abstract void SetUp();
        public abstract void CleanUp();
    }

    public interface IDelaySceneLoading
    {
        void SetUp();
        void CleanUp();
        bool IsDone { get; }
    }
}