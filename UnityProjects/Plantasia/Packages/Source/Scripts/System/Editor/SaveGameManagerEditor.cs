using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveGameManager))]
public class SaveGameManagerEditor : Editor
{
    // -------------------------------------------------------------------------------

    public override void OnInspectorGUI()
    {
        SaveGameManager saveGameManager = (SaveGameManager)target;
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Save Game"))
            {
                saveGameManager.Save();
            }

            if (GUILayout.Button("Reset Save Game"))
            {
                saveGameManager.ResetSaveGame();
            }
        }
    }

    // -------------------------------------------------------------------------------
}
