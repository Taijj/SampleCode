using Taijj.Input;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.InputSystem.Utilities;

public static class MapsEditor
{
    #region Main
    public static void DrawInspector(SerializedObject serializedObject)
    {
        if(Maps.Count == 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("InputActions Asset does not contain any maps. Please add some then return here!",
                MessageType.Error);
            return;
        }

        MapsProperty = serializedObject.FindProperty("_maps");
        EditorGUILayout.PropertyField(MapsProperty);

        if (false == MapsProperty.isExpanded)
            EditorGUILayout.Space();

        if (GUILayout.Button("Generate Map Scripts"))
            GenerateMapScripts();

        if (GUILayout.Button("Generate Map Assets"))
            GenerateMapAssets();
    }

    private static SerializedProperty MapsProperty { get; set; }
    #endregion



    #region Scrots Generation
    private const string TEMPLATE_NAME = "TemplateMap";

    private const string NAMESPACE_PLACERHOLDER = "{NAMESPACE}";
    private const string NAME_PLACEHOLDER = "{NAME}";
    private const string BASE_PLACEHOLDER = "{BASE}";

    private static void GenerateMapScripts()
    {
        string guid = AssetDatabase.FindAssets(TEMPLATE_NAME)[0];
        string templatePath = AssetDatabase.GUIDToAssetPath(guid).ToSystemPath();
        string templateContent = File.ReadAllText(templatePath);
        string folder = GetScriptsFolder();

        for (int i = 0; i < Maps.Count; i++)
            CreateScriptFor(Maps[i], templateContent, folder);

        AssetDatabase.Refresh();
        Note.Log($"Map Scripts generated at {folder}.", ColorAddons.Orange);
    }

    private static string GetScriptsFolder()
    {
        AssetImporter imp = AssetImporter.GetAtPath(ActionsAssetEditor.AssetPath);
        SerializedObject impOb = new SerializedObject(imp);
        string scriptPath = impOb.FindProperty("m_WrapperCodePath").stringValue;
        return scriptPath.Replace(StringAddons.SLASH + Model.GENERATED_SCRIPT, string.Empty);
    }

    private static void CreateScriptFor(InputActionMap map, string templateContent, string folder)
    {
        string file = folder + Path.DirectorySeparatorChar + $"{map.name}Map.cs";
		if (File.Exists(file.ToSystemPath()))
			return;

        StringBuilder builder = new StringBuilder(templateContent);
        builder.Replace(NAMESPACE_PLACERHOLDER, EditorSettings.projectGenerationRootNamespace);
        builder.Replace(NAME_PLACEHOLDER, map.name);
        builder.Replace(BASE_PLACEHOLDER, Model.GENERATED_CLASS);

        File.WriteAllText(file, builder.ToString());
        AssetDatabase.ImportAsset(file);
    }
    #endregion



    #region Assets
    private const string ASSETS_FOLDER_NAME = "MapAssets";

    private static void GenerateMapAssets()
    {
        string folder = ModelEditor.FolderPath + Path.DirectorySeparatorChar + ASSETS_FOLDER_NAME;
        if (false == Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        MapsProperty.arraySize = Maps.Count;
        for (int i = 0; i < Maps.Count; i++)
        {
            InputActionMap map = Maps[i];
            Type type = TypeAddons.Find($"{map.name}Map");
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, Path.Combine(folder, $"{map.name}.asset"));

            MapsProperty.GetArrayElementAtIndex(i).objectReferenceValue = asset;
        }
    }

    private static ReadOnlyArray<InputActionMap> Maps => ActionsAssetEditor.Asset.actionMaps;
    #endregion
}