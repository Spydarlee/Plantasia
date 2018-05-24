using UnityEngine;

[CreateAssetMenu(fileName = "GrowthReq_Water", menuName = "Plant Growth Reqs/Water", order = 1)]
public class WaterRequirement : RechargablePlantGrowthRequirement
{
    // This requirement will be shared among many instance so although we keep
    // a mCurrentCharge in the base class for running checks against,
    // we must set it to the current plant.WaterCharge value each time we do anything

    // -------------------------------------------------------------------------------

    public override bool CheckRequirement(Plant plant)
    {
        mCurrentCharge = plant.WaterCharge;
        UpdateCurrentCharge(plant.IsBeingRainedOn);

        plant.WaterCharge = mCurrentCharge;
        return base.CheckRequirement(plant);
    }

    // -------------------------------------------------------------------------------

    public override float GetStatusAsPercentage(Plant plant)
    {
        mCurrentCharge = plant.WaterCharge;
        return base.GetStatusAsPercentage(plant);
    }

    // -------------------------------------------------------------------------------
}