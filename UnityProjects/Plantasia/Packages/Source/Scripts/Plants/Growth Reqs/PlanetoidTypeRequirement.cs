using UnityEngine;

[CreateAssetMenu(fileName = "GrowthReq_PlanetoidType", menuName = "Plant Growth Reqs/Planetoid Type", order = 1)]
public class PlanetoidTypeRequirement : PlantGrowthRequirement
{
    // -------------------------------------------------------------------------------

    public Planetoid.PlanetoidTypes PlanetoidType = Planetoid.PlanetoidTypes.Default;

    // -------------------------------------------------------------------------------

    public override bool CheckRequirement(Plant plant)
    {
        return (Universe.Instance.CurrentPlanetoid.PlanetoidType == PlanetoidType);
    }

    // -------------------------------------------------------------------------------

    public override float GetStatusAsPercentage(Plant plant)
    {
        var meetsRequirement = CheckRequirement(plant);
        return (meetsRequirement) ? 1.0f : 0.0f;
    }

    // -------------------------------------------------------------------------------
}
