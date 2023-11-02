using System;
using UnityEngine;

/// <summary>
/// Turns a single or array of strings into a searchable key field. Use this, if you want to only allow
/// assignments of certain values from a list of existing string keys, e.g. localization or audio.
///
/// Note: An appropriate <see cref="KeySelectionSearchProvider">KeySelectionSearchProvider</see> needs to
/// be implemented to provide the corresponding list of keys.
///
/// Based on: https://www.youtube.com/watch?v=0HHeIUGsuW8
/// </summary>
public class KeySelectionAttribute : PropertyAttribute
{
    public KeySelectionAttribute(Type providerType)
    {
        if (providerType.IsSubclassOf(typeof(KeySelectionSearchProvider)))
            ProviderType = providerType;
        else
            Note.LogError($"{providerType.Name} must inherit from KeySelectionSearchProvider!");
    }

    public Type ProviderType { get; set; }
}