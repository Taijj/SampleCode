
using Taijj.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public static class ActionsAssetEditor
{
    #region Loading
    public static void TryLoadAsset()
    {
        if (false == ModelEditor.HasModel)
            return;

        AssetPath = ModelEditor.FolderPath + Path.DirectorySeparatorChar + Model.INPUT_ASSET_FILE;
        Asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
        HasAsset = Asset.HasReference();
    }

    public static string AssetPath { get; private set; }
    public static InputActionAsset Asset { get; private set; }

    public static bool HasAsset { get; private set; }
    #endregion



    #region Inspector
    public static void DrawInspector(SerializedObject serializedObject)
    {
        if (false == ModelEditor.HasModel)
            return;

        if (HasAsset)
            DrawWithAsset(serializedObject);
        else
            DrawWithoutAsset();
    }

    private static void DrawWithAsset(SerializedObject serializedObject)
    {
        GUI.enabled = false;
        SerializedProperty assetProp = serializedObject.FindProperty("_asset");
        assetProp.objectReferenceValue = Asset;
        EditorGUILayout.ObjectField(assetProp);
        GUI.enabled = true;
    }

    private static void DrawWithoutAsset()
    {
        string help = "Please create an InputActionsAsset!\n\n" +
            $"It needs to be named {Model.INPUT_ASSET_FILE} and should be " +
            $"in the folder {ModelEditor.FolderPath}! Use the Button below to automatcially create " +
            $"an appropriate Asset and accompanying scripts.";

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(help, MessageType.Error);

        EditorGUILayout.Space();
        if (GUILayout.Button("Create InputActions"))
            CreateInputActionsAsset();
    }
    #endregion



    #region Asset
    private const string DEFAULT_CONTENT = "{}";

    private static void CreateInputActionsAsset()
    {
        CreateAsset();
        ConfigureImporter();
    }

    private static void CreateAsset()
    {
        AssetPath = ModelEditor.FolderPath + Path.DirectorySeparatorChar + Model.INPUT_ASSET_FILE;
        string fullPath = Path.GetFullPath(AssetPath);
        File.WriteAllText(fullPath, DEFAULT_CONTENT.ToOsLineEndings());
        AssetDatabase.ImportAsset(AssetPath);
        Asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);

        Note.Log($"InputActionsAsset created at {AssetPath}", ColorAddons.Aqua);
    }

    private static void ConfigureImporter()
    {
        string scriptFolder = EditorUtility.OpenFolderPanel("InputActions Script Folder",
                EditorAddons.GetProjectViewFolder(),
                StringAddons.EMPTY);

        if(string.IsNullOrEmpty(scriptFolder))
        {
            string warning = "You've canceled the Folder Selection Dialog!" +
                $"the InputActions Script will then be generated in the same folder as the InputActions Asset!";

            scriptFolder = ModelEditor.FolderPath;
            Note.Log(warning, ColorAddons.Orange);
        }
        else
        {
            scriptFolder = scriptFolder.ToAssetPath();
        }

        AssetImporter imp = AssetImporter.GetAtPath(AssetPath);
        SerializedObject impOb = new SerializedObject(imp);
        SerializedProperty isGeneratedProp = impOb.FindProperty("m_GenerateWrapperCode");
        SerializedProperty filePathProp = impOb.FindProperty("m_WrapperCodePath");
        SerializedProperty classProp = impOb.FindProperty("m_WrapperClassName");
        SerializedProperty namespaceProp = impOb.FindProperty("m_WrapperCodeNamespace");

        isGeneratedProp.boolValue = true;
        filePathProp.stringValue = scriptFolder + StringAddons.SLASH + Model.GENERATED_SCRIPT;
        classProp.stringValue = Model.GENERATED_CLASS;
        namespaceProp.stringValue = EditorSettings.projectGenerationRootNamespace;
        impOb.ApplyModifiedProperties();
    }
    #endregion
}