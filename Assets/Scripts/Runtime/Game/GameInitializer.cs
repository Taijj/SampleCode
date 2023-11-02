using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taijj.HeartWarming
{
    /// <summary>
    /// Scene independently initializes the Game Scene and triggers <see cref="Game"/> initialization
    /// </summary>
    public static class GameInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            int additiveIndex = Sanitize();
            AsyncOperation op = SceneManager.LoadSceneAsync(additiveIndex, LoadSceneMode.Additive);
            op.completed += OnGameLoaded;
        }

        private static void OnGameLoaded(AsyncOperation operation)
        {
            operation.completed -= OnGameLoaded;
            Game game = UnityEngine.Object.FindObjectOfType<Game>();
            if (game.IsNull())
                throw new Exception("Game cannot be found! Please ensure the Game Scene is setup properly!");
            game.Wake();
            game.SetUp();
        }

        private static int Sanitize()
        {
            if (SceneManager.sceneCount > 1)
                throw new Exception("Having multiple Scenes loaded on startup is not supported!");

			int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
			return currentSceneBuildIndex == SceneModel.GAME_SCENE_BUILD_INDEX
				? SceneModel.FIRST_SCENE_BUILD_INDEX
				: SceneModel.GAME_SCENE_BUILD_INDEX;
        }
    }
}