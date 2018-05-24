using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // -------------------------------------------------------------------------------

    public List<Planetoid.PlanetoidTypes>                               UnlockedHomePlanetoids = new List<Planetoid.PlanetoidTypes>();
    public TutorialStages                                               TutorialStage = TutorialStages.NewGame;
    public List<PlanetoidAndPlantSaveDataList>                          HomePlanetoidPlants = new List<PlanetoidAndPlantSaveDataList>();
    public List<SeedSaveData>                                           SeedCollection = new List<SeedSaveData>();

    // -------------------------------------------------------------------------------

    public GameData()
    {
        // Default planetoid type is automatically unlocked!
        UnlockedHomePlanetoids.Add(Planetoid.PlanetoidTypes.Default);

        var defaultPlanetoidPlants = new PlanetoidAndPlantSaveDataList(Planetoid.PlanetoidTypes.Default);
        HomePlanetoidPlants.Add(defaultPlanetoidPlants);
    }

    // -------------------------------------------------------------------------------

    public bool IsHomePlanetoidTypeUnlocked(Planetoid.PlanetoidTypes planetoidTypeToCheck)
    {
        foreach (var planetoidType in UnlockedHomePlanetoids)
        {
            if (planetoidType == planetoidTypeToCheck)
            {
                return true;
            }
        }

        return false;
    }

    // -------------------------------------------------------------------------------

    public void UpdateHomePlanetoidPlants(Planetoid.PlanetoidTypes planetoidType, List<PlantSaveData> plants)
    {
        var planetoidAndPlants = HomePlanetoidPlants.FirstOrDefault(x => x.PlanetoidType == planetoidType);

        if (planetoidAndPlants != null)
        {
            var index = HomePlanetoidPlants.IndexOf(planetoidAndPlants);
            HomePlanetoidPlants[index].PlantSaveDataList = plants;
        }
        else
        {
            if (plants.Count > 0)
            {
                HomePlanetoidPlants.Add(new PlanetoidAndPlantSaveDataList(planetoidType, plants));
            }
        }
    }

    // -------------------------------------------------------------------------------
}
