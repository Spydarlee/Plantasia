using UnityEngine;

[CreateAssetMenu(fileName = "GrowthReq_Moonlight", menuName = "Plant Growth Reqs/Moonlight", order = 1)]
public class MoonlightRequirement : PlantGrowthRequirement
{
    // -------------------------------------------------------------------------------

    public override bool CheckRequirement(Plant plant)
    {
        return !Universe.Instance.TheSun.IsInSunlight(plant.transform.up);
    }

    // -------------------------------------------------------------------------------

    public override float GetStatusAsPercentage(Plant plant)
    {
        var meetsRequirement = CheckRequirement(plant);
        return (meetsRequirement) ? 1.0f : 0.0f;
    }

    // -------------------------------------------------------------------------------
}