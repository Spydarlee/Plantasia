using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------------------------------

[System.Serializable]
public class PlanetoidAndPlantSaveDataList
{
    // -------------------------------------------------------------------------------

    public Planetoid.PlanetoidTypes PlanetoidType = Planetoid.PlanetoidTypes.Default;
    public List<PlantSaveData> PlantSaveDataList = new List<PlantSaveData>();

    // -------------------------------------------------------------------------------

    public PlanetoidAndPlantSaveDataList(Planetoid.PlanetoidTypes planetoidType)
    {
        PlanetoidType = planetoidType;
        PlantSaveDataList = new List<PlantSaveData>();
    }

    // -------------------------------------------------------------------------------

    public PlanetoidAndPlantSaveDataList(Planetoid.PlanetoidTypes planetoidType, List<PlantSaveData> list)
    {
        PlanetoidType = planetoidType;
        PlantSaveDataList = list;
    }

    // -------------------------------------------------------------------------------
}

// -------------------------------------------------------------------------------

[System.Serializable]
public class PlantSaveData
{
    // -------------------------------------------------------------------------------

    public PlantTypes   PlantType = PlantTypes.Flower;
    public Vector3      Position = Vector3.zero;
    public Vector3      GroundNormal = Vector3.up;
    public float        NormalisedGrowth = 0.0f;

    // -------------------------------------------------------------------------------

    public PlantSaveData(Plant plant)
    {
        PlantType = plant.PlantType;
        Position = plant.transform.localPosition;
        GroundNormal = plant.GroundNormal;

        // Has the plant produced a seed that hasn't been collected yet?
        // If so, pretend the plant is still fully grown so it can be harvested
        // again next time (simpler than having to spawn another seed on load next time!)
        if (plant.SeedToBeCollected != null)
        {
            NormalisedGrowth = 1.0f;
        }
        else
        {
            NormalisedGrowth = plant.NormalisedGrowth;
        }
    }

    // -------------------------------------------------------------------------------
}

// -------------------------------------------------------------------------------

[System.Serializable]
public class SeedSaveData
{
    // -------------------------------------------------------------------------------

    public PlantTypes   PlantType = PlantTypes.Flower;
    public int          Count = 0;
}

// -------------------------------------------------------------------------------
