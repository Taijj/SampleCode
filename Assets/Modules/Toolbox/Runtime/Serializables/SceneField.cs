using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for injection of Scenes into the Unity inspector.
/// </summary>
[Serializable]
public class SceneField : IEquatable<SceneField>
{
    #region Main
    [SerializeField] private string _sceneName = default;
    #pragma warning disable 0414
    [SerializeField] private UnityEngine.Object _sceneAsset = default;
    #pragma warning restore

    public SceneField(string sceneName)
    {
        _sceneName = sceneName;
    }

    public string SceneName => _sceneName;
    #endregion



    #region Operators
    public override bool Equals(object obj)
    {
        return Equals(obj as SceneField);
    }

    public bool Equals(SceneField other)
    {
        return other != null && _sceneName == other._sceneName;
    }

    public override int GetHashCode()
    {
        return 232260920 + EqualityComparer<string>.Default.GetHashCode(_sceneName);
    }

    public static implicit operator string(SceneField sceneField) => sceneField.SceneName;
    #endregion
}