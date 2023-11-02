using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Taijj.Input
{
    public static class ModelEditor
    {
        #region Loading
        public static void TryLoadModel()
        {
            Model = EditorAddons.LoadSingle<Model>();
            HasModel = Model != null;
            if (false == HasModel)
                return;

            string assetPath = AssetDatabase.GetAssetPath(Model);
            int lastSlash = assetPath.LastIndexOf(StringAddons.SLASH);
            FolderPath = assetPath.Substring(0, lastSlash);
        }

        public static Model Model { get; private set; }
        public static string FolderPath { get; private set; }
        public static bool HasModel { get; private set; }
        #endregion



        #region Inspector
        public static void DrawInspector(SerializedObject serializedObject)
        {
            if (HasModel)
                DrawField(serializedObject);
            else
                DrawInfo();
        }

        private static void DrawInfo()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please create an InputModel!",
                MessageType.Error);

            EditorGUILayout.Space();
            if (GUILayout.Button("Create InputModel"))
                CreateAndInitializeModelAssets();
        }

        private static void DrawField(SerializedObject serializedObject)
        {
            GUI.enabled = false;
            SerializedProperty modelProp = serializedObject.FindProperty("_model");
            modelProp.objectReferenceValue = Model;
            EditorGUILayout.ObjectField(modelProp);
            GUI.enabled = true;
        }
        #endregion



        #region Asset Creation
        private const string MODEL_ASSET_FILE = "InputModel.asset";
        private const string PERIPHERALS_FOLDER_NAME = "Peripherals";

        private static void CreateAndInitializeModelAssets()
        {
            string parentFolder = EditorUtility.OpenFolderPanel("Model Creation",
                EditorAddons.GetProjectViewFolder(),
                StringAddons.EMPTY);

            if(string.IsNullOrEmpty(parentFolder))
                throw new Exception("Could not vreate Inpuzmodel, because the Folder Selection was canceled!");

            string assetFolder = parentFolder.ToAssetPath();
            CreatePlatforms(assetFolder);
            CreateModel(assetFolder);

            AssetDatabase.Refresh();
        }

        private static void CreatePlatforms(string parentFolder)
        {
            string folder = parentFolder + Path.DirectorySeparatorChar + PERIPHERALS_FOLDER_NAME;
            if (false == Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            Type[] subTypes = TypeAddons.GetSubtypesOf(typeof(Peripheral));
            Platforms = new Peripheral[subTypes.Length];
            for (int i = 0; i < subTypes.Length; i++)
            {
                Type subType = subTypes[i];
                string fileName = $"{subType.Name}.asset";
                string fullPath = folder + Path.DirectorySeparatorChar + fileName;

                ScriptableObject platform = ScriptableObject.CreateInstance(subType);
                AssetDatabase.CreateAsset(platform, fullPath);
                Platforms[i] = platform as Peripheral;
            }
        }

        private static void CreateModel(string parentFolder)
        {
            string fullPath = parentFolder + Path.DirectorySeparatorChar + MODEL_ASSET_FILE;
            Model model = ScriptableObject.CreateInstance<Model>();
            model.AssignDefaults(Platforms);

            AssetDatabase.CreateAsset(model, fullPath);
            TryLoadModel();
        }

        private static Peripheral[] Platforms { get; set; }
        #endregion
    }
}