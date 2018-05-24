using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : Orbitable
{
    // -------------------------------------------------------------------------------

    public AnimationCurve   DayNightBlendCurve = new AnimationCurve();
    public float            MinDotProductToBeInSunlight = 0.13f;

    // -------------------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();

        // Update the skybox based on the time of day (sun angle)
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_DayOrNightBlend", GetDayNightBlend());
        }
    }

    // -------------------------------------------------------------------------------

    public float GetDayNightBlend()
    {
        return DayNightBlendCurve.Evaluate(mDirectionAngle);
    }

    // ------------------------------------------------------------------------------

    public bool IsInSunlight(Vector3 normal)
    {
        var toSun = (transform.position - mCenterOfTheUniverse).normalized;
        return Vector2.Dot(normal, toSun) > MinDotProductToBeInSunlight;
    }

    // ------------------------------------------------------------------------------
}
