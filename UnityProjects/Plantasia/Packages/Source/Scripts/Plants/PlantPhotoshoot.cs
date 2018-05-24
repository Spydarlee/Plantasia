#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PlantPhotoshoot : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    private void Start()
    {
        string folderPath = EditorUtility.SaveFolderPanel("Desktop", "", "Screenshot.png");
        StartCoroutine(TakeScreenshots(folderPath));
    }

    // -------------------------------------------------------------------------------

    private IEnumerator TakeScreenshots(string folderPath)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        List<Transform> firstChildren = new List<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.parent == transform)
            {
                firstChildren.Add(child);
                child.gameObject.SetActive(false);
            }
        }

        foreach (var child in firstChildren)
        {
            var filePath = folderPath + "/" + child.gameObject.name + ".png";

            child.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            ScreenCapture.CaptureScreenshot(filePath);
            yield return new WaitForSeconds(0.5f);

            child.gameObject.SetActive(false);
        }
    }

    // -------------------------------------------------------------------------------
}

#endif