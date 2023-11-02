using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Taijj.Input
{
    [CustomEditor(typeof(UserInput))]
    public class UserInputEditor : Editor
    {
        public void OnEnable()
        {
            ModelEditor.TryLoadModel();
            ActionsAssetEditor.TryLoadAsset();
            TrySetupComponents();
        }

        private void TrySetupComponents()
        {
            UserInput input = target as UserInput;
            SerializedProperty eventProp = serializedObject.FindProperty("_eventSystem");
            if (eventProp.objectReferenceValue.IsNull(true))
            {
                if (false == input.TryGetComponent(out EventSystem system))
                {
                    system = input.gameObject.AddComponent<EventSystem>();
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(system);
                }

                eventProp.objectReferenceValue = system;
            }

            SerializedProperty modProp = serializedObject.FindProperty("_uiModule");
            if (modProp.objectReferenceValue.IsNull(true))
            {
                if (false == input.TryGetComponent(out InputSystemUIInputModule module))
                {
                    module = input.gameObject.AddComponent<InputSystemUIInputModule>();
                    module.actionsAsset = ActionsAssetEditor.Asset;
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(module);
                }

                modProp.objectReferenceValue = module;
            }

            serializedObject.ApplyModifiedProperties();
        }



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            ModelEditor.DrawInspector(serializedObject);
            ActionsAssetEditor.DrawInspector(serializedObject);

            bool hasConfiguration = ModelEditor.HasModel && ActionsAssetEditor.HasAsset;
            if (hasConfiguration)
            {
                EditorGUILayout.Space();
                MapsEditor.DrawInspector(serializedObject);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}