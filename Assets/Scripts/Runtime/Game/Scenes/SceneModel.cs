using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Contains statically accessible information about the currently focused Scene.
	/// </summary>
	[CreateAssetMenu(fileName = "SceneModel", menuName = "Ble/Heartwarming/SceneModel")]
	public class SceneModel : ScriptableObject
    {
		#region Main
		[SerializeField] private SceneField[] _scenesInOrder;



		private static SceneModel Instance { get; set; }
		public static void Wake(SceneModel instance) => Instance = instance;



		public static void Refresh(SceneRoot root)
        {
			CurrentRoot = root;
            Current = GetCurrentScene();
        }

        private static Scene GetCurrentScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene sc = SceneManager.GetSceneAt(i);
                if (sc.buildIndex == GAME_SCENE_BUILD_INDEX)
                    continue;

                return sc;
            }

			Note.LogError("Current Scene not found!");
			return SceneManager.GetActiveScene();
        }
		#endregion



		#region Info
		public static int GAME_SCENE_BUILD_INDEX = 0;
		public static int FIRST_SCENE_BUILD_INDEX = 1;

		public static Scene Current { get; private set; }
        public static SceneRoot CurrentRoot { get; private set; }
		#endregion



		#region Tools
		public static string GetSubsequentSceneName()
		{
			SceneField[] scenes = Instance._scenesInOrder;

			int nextIndex = Current.buildIndex; // "Game" Scene is ignored!
			if (nextIndex >= scenes.Length)
				throw new Exception("Subsequent Scene does not exist!");

			return scenes[nextIndex];
		}
		#endregion
	}
}