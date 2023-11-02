using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneAddons
{
    /// <summary>
    /// Get a List containing all components of the given Type in this Scene.
    /// Note: This is a rather heavy operation, use with care!
    /// </summary>
    /// <param name="estimatedCount">Sets the capacity of the resulting List. Use this to optimize memory allocations.</param>
    /// <returns></returns>
    public static List<T> GetComponents<T>(this Scene @this, bool includeInactive = true, int estimatedCount = 10)
        where T : Component
    {
        List<T> result = new List<T>(estimatedCount);
        GameObject[] objects = @this.GetRootGameObjects();
        for(int i = 0; i< objects.Length; i++)
            result.AddRange(objects[i].GetComponentsInChildren<T>(true));
        return result;
    }
}