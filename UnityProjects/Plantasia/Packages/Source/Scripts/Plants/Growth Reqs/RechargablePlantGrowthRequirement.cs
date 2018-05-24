using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargablePlantGrowthRequirement : PlantGrowthRequirement
{
    public float        MinimumCharge = 0.0f;
    public float        MaximumCharge = 10.0f;
    public float        RechargeRate = 1.0f;
    public float        FalloffRate = 1.0f;
    protected float     mCurrentCharge = 0.0f;

    // -------------------------------------------------------------------------------

    public override bool CheckRequirement(Plant plant)
    {
        return mCurrentCharge > MinimumCharge;
    }

    // -------------------------------------------------------------------------------

    public override float GetStatusAsPercentage(Plant plant)
    {
        return (mCurrentCharge / MinimumCharge);
    }

    // -------------------------------------------------------------------------------

    protected void UpdateCurrentCharge(bool isRecharging)
    {
        if (isRecharging)
        {
            mCurrentCharge += RechargeRate * Time.deltaTime;
        }
        else
        {
            mCurrentCharge -= FalloffRate * Time.deltaTime;
        }

        mCurrentCharge = Mathf.Clamp(mCurrentCharge, 0.0f, MaximumCharge);
    }

    // -------------------------------------------------------------------------------
}