using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple Helper for outputting logs on Screen.
/// </summary>
public class GuiConsole : MonoBehaviour
{
    #region LifeCycle
    private const string STYLE_TEMPLATE = "textArea";

    [SerializeField] private Vector2 _windowSize = new Vector2(0.6f, 0.2f);
    [SerializeField] private Vector2 _entrySize = new Vector2(0.5f, 0.05f);
    [SerializeField] private float _margin = 0.01f;

    public void Wake()
    {
        Entries = new List<string>();
        IsReady = true;
    }
    #endregion



    #region Logging
    public void OnGUI()
    {
        if (false == IsReady)
            return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector2 consoleSize = new Vector2(screenWidth * _windowSize.x, screenHeight * _windowSize.y);
        Vector2 entrySize = new Vector2(screenWidth * _entrySize.x, screenHeight * _entrySize.y);
        float margin = Screen.height * _margin;

        float entryHeight = entrySize.y + margin;
        Rect scrollRect = new Rect(margin, screenHeight - consoleSize.y - margin, consoleSize.x, consoleSize.y);
        Rect viewRect = new Rect(margin, margin, consoleSize.x - margin*4f, entryHeight * Entries.Count);

        if (EntryWasAdded)
            ScrollPosition = Vector2.up * viewRect.height;
        ScrollPosition = GUI.BeginScrollView(scrollRect, ScrollPosition, viewRect);

        GUIStyle style = new GUIStyle(STYLE_TEMPLATE);
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = (int)(entrySize.y / 2f);
        for (int i = 0; i < Entries.Count; i++)
            GUI.TextArea(new Rect(viewRect.x, viewRect.y + i * entryHeight, entrySize.x, entrySize.y), Entries[i], style);

        GUI.EndScrollView();
        EntryWasAdded = false;
    }

    public void Log(string message)
    {
        Entries.Add(message);
        EntryWasAdded = true;
    }



    private List<string> Entries { get; set; }
    private Vector2 ScrollPosition { get; set; }
    private bool EntryWasAdded { get; set; }
    private bool IsReady { get; set; }
    #endregion
}