using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrowthReq_Sunlight", menuName = "Plant Growth Reqs/Sunlight", order = 1)]
public class SunlightRequirement : PlantGrowthRequirement
{
    // -------------------------------------------------------------------------------

    public override bool CheckRequirement(Plant plant)
    {
        return Universe.Instance.TheSun.IsInSunlight(plant.transform.up);
    }

    // -------------------------------------------------------------------------------

    public override float GetStatusAsPercentage(Plant plant)
    {
        var meetsRequirement = CheckRequirement(plant);
        return (meetsRequirement) ? 1.0f : 0.0f;
    }

    // -------------------------------------------------------------------------------
}