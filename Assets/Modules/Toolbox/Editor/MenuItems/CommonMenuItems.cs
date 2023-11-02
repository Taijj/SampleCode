using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CommonMenuItems
{
    #region Project
    [MenuItem("Tools/Ble/Project/ClearLocalSavegames")]
    public static void ClearLocalSavegames()
    {
        PlayerPrefs.DeleteAll();
        Directory.Delete(Application.persistentDataPath, true);
    }

    [MenuItem("Tools/Ble/Project/Count selected objects")]
    public static void CountSelectedObjects()
    {
        Note.Log($"{Selection.objects.Length} are selected.");
    }

    [MenuItem("Tools/Ble/Project/Play from beginning")]
    public static void PlayFromBeginning()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // Scene 0 is expected to be a management Scene, e.g. "Game"
        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[1].path);
        EditorApplication.isPlaying = true;
    }
    #endregion



    #region Assets
    [MenuItem("Tools/Ble/Assets/Update .meta files")]
    public static void UpdateMetaFiles()
    {
        AssetDatabase.ForceReserializeAssets();
    }
    #endregion
}

