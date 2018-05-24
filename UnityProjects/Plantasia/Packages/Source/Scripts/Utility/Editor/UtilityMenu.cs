using UnityEngine;
using UnityEditor;

public static class UtilityMenu
{
    // -------------------------------------------------------------------------------

    [MenuItem("Utility/Capture Screenshot")]
    public static void CaptureScreenshot()
    {
        string path = EditorUtility.SaveFilePanel("Screenshot Destination", "", "Screenshot", "png");
        ScreenCapture.CaptureScreenshot(path);
    }

    // -------------------------------------------------------------------------------
}