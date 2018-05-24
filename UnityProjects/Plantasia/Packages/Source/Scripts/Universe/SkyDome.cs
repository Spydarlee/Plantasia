using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDome : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public float            TextureOffsetY = 0.5f;
    public Sun              ActiveSun = null;
    public AnimationCurve   SunAngleToTextureOffsetCurve = new AnimationCurve();

    // -------------------------------------------------------------------------------

    private Renderer        mRenderer;
    private Vector2         mTextureOffset = new Vector2(0.0f, 0.5f);

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        mRenderer = GetComponent<Renderer>();
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        if (ActiveSun == null)
        {
            ActiveSun = GameObject.FindObjectOfType<Sun>();
        }
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        if (ActiveSun)
        {
            mTextureOffset.x = SunAngleToTextureOffsetCurve.Evaluate(ActiveSun.DirectionAngle);
            mRenderer.material.SetTextureOffset("_MainTex", mTextureOffset);
        }        
    }

    // -------------------------------------------------------------------------------
}
