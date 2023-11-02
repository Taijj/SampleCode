using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.U2D.PSD;
using System.Linq;

// References:
// https://forum.unity.com/threads/psd-importer-and-secondary-textures.818595/
// https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
public class ExtractPsbLayout : EditorWindow
{
	#region Main
	private const string MENU_PATH = "Assets/Ble/ExtractLayout";

	[MenuItem(MENU_PATH, isValidateFunction: true)]
	public static bool IsValid()
	{
		Object[] sel = Selection.objects;
		if (sel.Length == 0)
			return false;

		Importers = new List<PSDImporter>();
		for(int i = 0; i < sel.Length; i++)
		{
			string path = AssetDatabase.GetAssetPath(sel[i]);
			AssetImporter importer = AssetImporter.GetAtPath(path);

			if(importer is PSDImporter)
				Importers.Add(importer as PSDImporter);
		}

		return Importers.Count > 0;
	}

	[MenuItem(MENU_PATH)]
	public static void FindAtlasses()
	{
		if (false == IsValid())
			return;

		DetermineDestinationPath(out bool isSuccessful);
		if (isSuccessful)
		{
			CollectLayoutTextures();
			ExtractTextures();

			EditorUtility.OpenWithDefaultApp(DestinationPath);
		}
	}

	private static List<PSDImporter> Importers { get; set; }
	#endregion



	#region Save Location
	private const string EDITOR_PREFS_KEY = "LastSuccessfulPsbLayoutsPath";
	private const string IN_PROJECT_ERROR = "The resulting files are duplicates of existing project Assets, and " +
		"therefore should be saved OUTSIDE the project hierarchy!\n\n" +
		"To prevent unnecessarily blowing up remote storage size, please retry and select a folder OUTSIDE of " +
		"the project hierarchy!";

	private static void DetermineDestinationPath(out bool isSuccessful)
	{
		string previousPath = EditorPrefs.GetString(EDITOR_PREFS_KEY);
		string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

		DestinationPath = EditorUtility.OpenFolderPanel("Select Destination Folder",
				string.IsNullOrEmpty(previousPath) ? desktopPath : previousPath,
				StringAddons.EMPTY);

		if(string.IsNullOrEmpty(DestinationPath))
		{
			isSuccessful = false;
			return;
		}
		EditorPrefs.SetString(EDITOR_PREFS_KEY, DestinationPath);

		// To prevent errors due to wrong directory separator characters, two paths are needed.
		string projectPath = Directory.GetCurrentDirectory();
		string formatted = Path.GetFullPath(DestinationPath);
		if (formatted.Contains(projectPath))
		{
			EditorUtility.DisplayDialog("Invalid Folder!", IN_PROJECT_ERROR, "Ok");
			isSuccessful = false;
			return;
		}

		isSuccessful = true;
	}

	private static string DestinationPath { get; set; }
	#endregion



	#region Textures
	private static void CollectLayoutTextures()
	{
		LayoutTextures = new List<Texture2D>();
		for (int i = 0; i < Importers.Count; i++)
		{
			string path = Importers[i].assetPath;
			string assetName = path.Substring(path.LastIndexOf(StringAddons.SLASH)+1)
				.Replace(".psb", StringAddons.EMPTY);

			LayoutTextures.Add(AssetDatabase.LoadAllAssetsAtPath(path)
				.OfType<Texture2D>()
				.First(t => t.name == assetName));
		}
	}

	private static void ExtractTextures()
	{
		for (int i = 0; i < LayoutTextures.Count; i++)
			Extract(LayoutTextures[i], DestinationPath);
	}

	private static List<Texture2D> LayoutTextures { get; set; }
	#endregion



	#region Extraction
	public static void Extract(Texture2D texture, string folderPath)
    {
        Texture2D copy = Duplicate(texture);
        byte[] _bytes = copy.EncodeToPNG();

		string path = Path.Join(folderPath, $"{texture.name}.png");
        File.WriteAllBytes(path, _bytes);
        AssetDatabase.Refresh();
    }

	/// <summary>
	/// Because it's not possible to set the read/write flags directly on PSB files,
	/// we need to temporarily create our own texture for pixel reading/writing.
	/// </summary>
	private static Texture2D Duplicate(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.sRGB);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
	#endregion
}
