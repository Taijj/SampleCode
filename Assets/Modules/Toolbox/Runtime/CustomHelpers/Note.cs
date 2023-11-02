#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_ENABLED
#endif

using System.Reflection;
using UnityEngine;

/// <summary>
/// Wrapper and Extension for Unity's Debug.Log() features. Prevents accidental
/// Logs from appearing in production builds.
/// </summary>
public static class Note
{
    #region Default Logging
    const string COLOR_TAG_FORMAT = "<color=#{0}>{1}</color>";

    public static void Log(object message, Object context = null)
    {
        #if IS_ENABLED
            Debug.Log(message, context);
        #endif
    }

    public static void Log(object message, Color color, Object context = null)
    {
        #if IS_ENABLED
            Debug.Log(string.Format(COLOR_TAG_FORMAT, ColorUtility.ToHtmlStringRGB(color), message), context);
        #endif
    }

    public static void LogWarning(object message, Object context = null)
    {
        #if IS_ENABLED
            Debug.LogWarning(message, context);
        #endif
    }

    public static void LogError(object message, Object context = null)
    {
        #if IS_ENABLED
            Debug.LogError(message, context);
        #endif
    }


    /// <summary>
    /// Logs "Boop!" in Magenta to the Console. Use this for quick debugging, e.g. like checking, if a certain code was executed.
    /// </summary>
    public static void Boop() => Log("Boop!", Color.magenta);

    /// <summary>
    /// Logs "TODO: {message}" in red to the Console. Use this to remind yourself, to implement features that still need to be implemented.
    /// </summary>
    public static void Todo(string message) => Log(string.IsNullOrEmpty(message) ? "TODO!" : $"TODO: {message}", Color.red);

    /// <summary>
    /// Logs "TODO!" in red to the Console. Use this to remind yourself, to implement features that still need to be implemented.
    /// </summary>
    public static void Todo() => Todo(string.Empty);
    #endregion



    #region Debug Drawing
    public static void DrawRay(Vector3 origin, Vector3 direction, Color color, float duration = -1f)
    {
        #if IS_ENABLED
            Debug.DrawRay(origin, direction, color, duration);
        #endif
    }

    public static void DrawCross(Vector3 center, float radius, Color color, float duration = -1f)
    {
        #if IS_ENABLED
            if (duration == -1f)
                duration = Time.deltaTime;
            Debug.DrawLine(center + Vector3.up * radius, center, color, duration);
            Debug.DrawLine(center - Vector3.up * radius, center, color, duration);
            Debug.DrawLine(center + Vector3.right * radius, center, color, duration);
            Debug.DrawLine(center - Vector3.right * radius, center, color, duration);
        #endif
    }

    public static void DrawBox(Vector3 center, Vector3 size, Color color, float duration = -1f)
    {
        #if IS_ENABLED
            Vector3 topLeft = center + new Vector3(-size.x, size.y) / 2f;
            Vector3 topRight = center + size / 2f;
            Vector3 bottomRight = center + new Vector3(size.x, -size.y) / 2f;
            Vector3 bottomLeft = center - size / 2f;

            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
        #endif
    }

    public static void DrawCircle(Vector3 center, float radius, Color color, float duration = -1f, int granularity = 12)
    {
        #if IS_ENABLED
            float angle = 360f / 12;
            Vector3 lastPoint = center + Vector3.right * radius;
            for (int i = 1; i <= 12; i++)
            {
                Vector3 nextPoint = center + radius * new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0f);
                Debug.DrawLine(lastPoint, nextPoint, color, duration);
                lastPoint = nextPoint;
            }
        #endif
    }
    #endregion



    #region Console
    /// <summary>
    /// Clears the Unity Console.
    /// </summary>
    public static void Clear()
    {
        #if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            System.Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        #endif
    }

    /// <summary>
    /// Logs the given message for any build on any platform.
    /// </summary>
    public static void ForceLog(object message)
    {
        Debug.Log(message); // For Player Log
        System.Console.WriteLine(message); // For System console, e.g. PS5
    }
    #endregion
}
