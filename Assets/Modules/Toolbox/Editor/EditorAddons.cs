using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper class for often used Editor features.
/// </summary>
public static class EditorAddons
{
    #region Misc
	/// <summary>
	/// Use this, if you want to know, if the property is part of an array.
	/// Unity's .isArray does not return the right values!
	/// </summary>
	public static bool IsArray(this SerializedProperty @this)
    {
        return @this.propertyPath.EndsWith(']');
    }

    /// <summary>
    /// Returns the path to the currently active Folder in the Project View.
    /// </summary>
    public static string GetProjectViewFolder()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        return obj.ToString();
    }

	/// <summary>
	/// Returs true, if the Editor can switch to the given build target, false otherwise.
	/// Taken from: https://answers.unity.com/questions/1324195/detect-if-build-target-is-installed.html
	/// </summary>
	public static bool IsAvailable(BuildTarget target)
	{
		Type moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
		MethodInfo isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
		MethodInfo getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", BindingFlags.Static | BindingFlags.NonPublic);

		string targetString = (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target });
		return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { targetString });
	}
	#endregion



	#region Assets
	/// <summary>
	/// Loads a single Asset of the given Type from the project.
	/// </summary>
	public static T LoadSingle<T>() where T : UnityEngine.Object
    {
        string typeName = typeof(T).FullName;
        string search = $"t:{typeName}";

        string[] guids = AssetDatabase.FindAssets(search);
        if (guids.Length == 0)
        {
            Note.LogWarning($"Tried to load {typeName}, but couldn't find any in the project! Please add one!");
            return default;
        }

        if (guids.Length > 1)
            Note.LogWarning($"More than one instance of {typeName} detected in the project! Returning the first one found!");

        string guid = guids[0];
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    /// <summary>
    /// Loads all Assets of the given Type from the project.
    /// </summary>
    public static T[] LoadAll<T>() where T: UnityEngine.Object
    {
        string search = $"t:{typeof(T).Name}";
        return AssetDatabase.FindAssets(search)
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<T>(p))
            .ToArray();
    }
	#endregion



	#region Layouting
	public static float SingleLineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

	public static GUIStyle AreaStyle
	{
		get
		{
			GUIStyle style = new GUIStyle(GUI.skin.GetStyle("window"));
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 13;
			style.alignment = TextAnchor.UpperLeft;
			style.padding = new RectOffset(12, 12, 30, 15);
			style.stretchHeight = false;

			return style;
		}
	}

	public static void BeginArea(string label, SerializedObject obj = null)
	{
		obj?.UpdateIfRequiredOrScript();

		GUILayout.BeginVertical(label, AreaStyle);
		GUILayout.Label(StringAddons.EMPTY, GUI.skin.horizontalSlider);
		GUILayout.Space(20);
	}

	public static void EndArea(SerializedObject obj = null)
	{
		GUILayout.EndVertical();
		obj?.ApplyModifiedProperties();
	}
	#endregion
}