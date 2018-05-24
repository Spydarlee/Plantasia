using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static GameCursor    Instance;
    public Texture2D            DefaultTexture = null;
    public Vector2              Hotspot = Vector2.zero;
    public float                TimeBetweenDoubleClicks = 0.3f;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        Cursor.SetCursor(DefaultTexture, Hotspot, CursorMode.ForceSoftware);
    }

    // ------------------------------------------------------------------------------
}
