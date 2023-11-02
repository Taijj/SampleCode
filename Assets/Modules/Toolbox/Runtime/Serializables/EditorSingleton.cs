using System;
using UnityEditor;
using UnityEngine;

public class EditorSingleton<T> : ScriptableObject where T:ScriptableObject
{
    #if UNITY_EDITOR
    private static T _editorInstance;
    public static T EditorInstance
    {
        get
        {
            if (_editorInstance.IsNull(true))
                _editorInstance = Load();
            return _editorInstance;
        }
    }

    private static T Load()
    {
        string typeName = typeof(T).Name;
        string[] guids = AssetDatabase.FindAssets($"t:{typeName}");
        if (guids.Length == 0)
            throw new SingletonException($"Tried to load {typeName}, but couldn't find any in the project! Please create one!", guids.Length);

        if (guids.Length > 1)
            throw new SingletonException($"More than one instance of {typeName} found in the project! Please delete all {typeName}s until only one is left!", guids.Length);

        string guid = guids[0];
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    #endif
}

public class SingletonException : Exception
{
	public SingletonException(string message, int instanceCount) : base(message)
	{
		InstanceCount = instanceCount;
	}

	public int InstanceCount { get; private set; }
}