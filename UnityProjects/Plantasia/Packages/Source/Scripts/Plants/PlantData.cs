using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Data", menuName = "Plant Data", order = 1)]
public class PlantData : ScriptableObject
{
    public PlantTypes                           PlantType = PlantTypes.Flower;
    public GameObject                           ModelPrefab = null;
    public Mesh                                 Mesh = null;
    public AnimatorOverrideController           AnimatorOverrideController = null;
    public int                                  NumGrowthAnims = 1;
    public float                                InitialGrowth = 0.3f;
    public float                                GrowthDuration = 10.0f;
    public Transform                            SeedTransform = null;
    public float                                BaseRadius = 0.5f;
    public List<PlantGrowthRequirement>         GrowthRequirements = new List<PlantGrowthRequirement>();
    public Sprite                               SeedSprite = null;

    // -------------------------------------------------------------------------------

    public bool CanGrowOnPlanetoid(Planetoid.PlanetoidTypes planetoidType)
    {
        foreach (var growthRequirement in GrowthRequirements)
        {
            var planetoidTypeReq = (growthRequirement as PlanetoidTypeRequirement);
            if (planetoidTypeReq != null && planetoidTypeReq.PlanetoidType != planetoidType)
            {
                return false;
            }
        }

        return true;
    }

    // -------------------------------------------------------------------------------
}

// -------------------------------------------------------------------------------
