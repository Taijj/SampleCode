using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Base for a list of strings that is to be shown in an organized, leveled search window.
///
/// To implement a provider for a specific set of keys, inherit from this class, and call
/// Initialize() in your class' constructor with the respective data.
/// </summary>
public class KeySelectionSearchProvider : ScriptableObject, ISearchWindowProvider
{
    #region Lifecycle
    /// <summary>
    /// Call this before using this provider.
    /// </summary>
    /// <param name="keys">The list of string keys, that can be selected by the user.</param>
    /// <param name="delimiter">Delimiter that separates each word of a key. E.g. in "location_hub" the Delimiter would be "_".</param>
    /// <param name="maxDepth">The number of levels the keys will be oragnized in in the search window. If set to 0, it will be a simple list.</param>
    protected virtual void Initialize(string[] keys, string delimiter = StringAddons.UNDESCORE, int maxDepth = 0)
    {
        Keys = keys;
        Delimiter = delimiter;
        MaxDepth = Mathf.Max(0, maxDepth);
    }

    public string[] Keys { get; private set; }
    private string Delimiter { get; set; }
    private int MaxDepth { get; set; }
    #endregion



    #region Search Tree
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<string> sortedKeys = SortKeys();
        return CreateTree(sortedKeys);
    }

    /// <summary>
    /// Creates a sorted list of Keys, so the SearchTree can be created easier.
    /// </summary>
    private List<string> SortKeys()
    {
        List<string> keys = Keys.ToList();
        keys.Sort((a, b) =>
        {
            string[] split1 = a.Split(Delimiter);
            string[] split2 = b.Split(Delimiter);
            for (int i = 0; i < split1.Length; i++)
            {
                if (i >= split2.Length)
                    return 1;

                int compareValue = split1[i].CompareTo(split2[i]);
                if (compareValue == 0)
                    continue;

                if (split1.Length == split2.Length)
                    return compareValue;

                // Puts Nodes before Leafs
                bool isLastIndex = i == split1.Length - 1 || i == split2.Length - 1;
                if (isLastIndex)
                    return split1.Length < split2.Length ? 1 : -1;
                return compareValue;
            }

            return 0;
        });

        return keys;
    }

    private List<SearchTreeEntry> CreateTree(List<string> keys)
    {
        List<SearchTreeEntry> result = new List<SearchTreeEntry>
        {
            // The first entry is always the healdine in the Search Window
            new SearchTreeGroupEntry(new GUIContent("Key Selection"))
        };

        List<string> groups = new List<string>();
        foreach (string key in keys)
        {
            string[] split = key.Split(Delimiter);
            int maxDepth = Mathf.Min(split.Length-1, MaxDepth);

            string groupPath = string.Empty;
            for (int i = 0; i < maxDepth; i++)
            {
                string current = split[i];
                groupPath += current;
                if (false == groups.Contains(groupPath))
                {
                    groups.Add(groupPath);
                    result.Add(new SearchTreeGroupEntry(new GUIContent(current), i+1));
                }
                groupPath += Delimiter;
            }

            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(key));
            entry.level = maxDepth+1;
            entry.userData = key; // Data handed to the callback when an option is selected.
            result.Add(entry);
        }

        return result;
    }
    #endregion



    #region Selection
    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        OnSelected?.Invoke((string)SearchTreeEntry.userData);
        return true;
    }

    public Action<string> OnSelected { set; get; }
    #endregion
}

#else

// Since Attributes survive until runtime, attempted builds fail, because ISearchWindowProvider resides in an
// Editor only namespace. To work around this, this empty implementation is used in builds instead.
public class KeySelectionSearchProvider : ScriptableObject
{
	protected virtual void Initialize(string[] keys, string delimiter = StringAddons.UNDESCORE, int maxDepth = 0)
	{}
}

#endif