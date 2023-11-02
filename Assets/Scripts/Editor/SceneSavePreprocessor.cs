
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Goes over all Scene objects and calls "OnValidate" on them,
	/// before the scene is saved.
	///
	/// This helps with preventing missing injections.
	/// </summary>
	public class SceneSavePreprocessor : AssetModificationProcessor
	{
		public static string[] OnWillSaveAssets(string[] paths)
		{
			Scene scene;
			string path;
			PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
			if (stage != null && stage.stageHandle.IsValid())
			{
				scene = stage.scene;
				path = stage.assetPath;
			}
			else
			{
				scene = SceneManager.GetActiveScene();
				path = scene.path;
			}

			if (paths.Contains(path))
				Preprocess(scene);
			return paths;
		}

		[ExecuteInEditMode]
		private static void Preprocess(Scene scene)
		{
			List<MonoBehaviour> behaviors = new List<MonoBehaviour>();
			foreach(GameObject root in scene.GetRootGameObjects())
				behaviors.AddRange(root.GetComponentsInChildren<MonoBehaviour>());

			foreach (MonoBehaviour b in behaviors)
			{
				MethodInfo info = b.GetType().GetMethod("OnValidate");
				if (info != null)
					info.Invoke(b, null);
			}
		}
	}
}