using UnityEngine;

public static class ScreenAddons
{
    /// <summary>
    /// This checks if the currently used displays are capable of
    /// HDR output. If they are, the game requests to disable HDR
    /// for each display, because the game won't display right, if
    /// HDR is enabled.
    ///
    /// Note: This is totally unrelated to Unity's HDR stuff!
    /// </summary>
    public static void TryDisableHDROutput()
    {
        HDROutputSettings[] displays = HDROutputSettings.displays;
        for (int i = 0; i < displays.Length; i++)
        {
            HDROutputSettings display = displays[i];
            bool canHdr = display.available;
            if (false == canHdr)
                continue;

            bool hasHdr = display.active;
            if (hasHdr)
                displays[i].RequestHDRModeChange(false);
        }
    }
}