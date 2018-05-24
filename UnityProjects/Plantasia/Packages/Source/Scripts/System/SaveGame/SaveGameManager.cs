using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static SaveGameManager Instance = null;

    // -------------------------------------------------------------------------------

    public GameData     SaveGameData = new GameData();
    public bool         DisableLoadingFromFile = false;
    public float        AutoSaveInterval = 15.0f;

    // -------------------------------------------------------------------------------

    public bool IsLoaded { get; set; }

    // -------------------------------------------------------------------------------

    public delegate void SaveGameLoadedAction(GameData gameData);
    public static event SaveGameLoadedAction OnSaveGameLoaded = null;

    // -------------------------------------------------------------------------------

    private BinaryFormatter mBinaryFormatter = null;
    private string          mSaveFileName = null;
    private float           mTimeSinceLastSave = 0.0f;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        Instance = this;
        IsLoaded = false;

        // Create the BinaryFormatter for saving and loading
        mBinaryFormatter = new BinaryFormatter();

        // By default, the BinaryFormatter can't (de)serialse Unity types like Vectors, so we have to use surrogates
        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        mBinaryFormatter.SurrogateSelector = surrogateSelector;

        if (!Application.isEditor)
        {
            SaveGameData = new GameData();
        }
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        // Load the savegame from the given file
        mSaveFileName = Application.persistentDataPath + "/savegame.plantasia";
        Load();
    }

    // -------------------------------------------------------------------------------

    private void Update()
    {
        mTimeSinceLastSave += Time.deltaTime;
        if (mTimeSinceLastSave >= AutoSaveInterval)
        {
            Save();
        }
    }

    // -------------------------------------------------------------------------------

    public void Save(bool updateSaveGameData = true)
    {
        Debug.Log("Saving game to " + mSaveFileName);
        mTimeSinceLastSave = 0.0f;

        if (updateSaveGameData)
        {
            // Grab data for all the plants that have been planted on the home planetoid       
            SaveHomePlanetoidPlants();
        }

        // Save out the data!
        FileStream file = File.Open(mSaveFileName, FileMode.OpenOrCreate);
        mBinaryFormatter.Serialize(file, SaveGameData);
        file.Close();
    }

    // -------------------------------------------------------------------------------

    public void Load()
    {
        if (!DisableLoadingFromFile || !Application.isEditor)
        {
            if (File.Exists(mSaveFileName))
            {
                Debug.Log("Loading savegame from " + mSaveFileName);

                FileStream file = File.Open(mSaveFileName, FileMode.Open);
                SaveGameData = (GameData)mBinaryFormatter.Deserialize(file);
                file.Close();
            }
            else
            {
                Debug.LogWarning("Could not find savegame at:" + mSaveFileName);
            }
        }
        else
        {
            Debug.Log("Ignoring request to load save game because DisableLoadingFromFile is set to true.");
        }

        LoadHomePlanetoidPlants();
        TutorialManager.Instance.CurrentStage = SaveGameData.TutorialStage;

        IsLoaded = true;
        if (OnSaveGameLoaded != null)
        {
            OnSaveGameLoaded.Invoke(SaveGameData);
        }
    }

    // -------------------------------------------------------------------------------

    public void ResetSaveGame()
    {
        SaveGameData = new GameData();
        Save(false);
    }

    // -------------------------------------------------------------------------------

    public void UnlockedHomePlanetoid(Planetoid.PlanetoidTypes planetoidType)
    {
        if (!SaveGameData.IsHomePlanetoidTypeUnlocked(planetoidType))
        {
            Debug.Log("Unlocking planetoid type: " + planetoidType);
            SaveGameData.UnlockedHomePlanetoids.Add(planetoidType);
            SaveGameData.HomePlanetoidPlants.Add( new PlanetoidAndPlantSaveDataList(planetoidType) );
            Save();
        }
        else
        {
            Debug.LogWarning("Tried to unlock planetoid type: " + planetoidType + ", but it's already unlocked!");
        }
    }

    // -------------------------------------------------------------------------------

    public void UpdateTutorialStage(TutorialStages newStage)
    {
        SaveGameData.TutorialStage = newStage;
        Save();
    }

    // -------------------------------------------------------------------------------

    public void SaveSeedCollection(SeedShip seedship)
    {
        SaveGameData.SeedCollection.Clear();

        foreach (PlantTypes plantType in System.Enum.GetValues(typeof(PlantTypes)))
        {
            var seedSaveData = new SeedSaveData
            {
                PlantType = plantType,
                Count = seedship.GetSeedCount(plantType)
            };

            SaveGameData.SeedCollection.Add(seedSaveData);
        }

        Save();
    }

    // -------------------------------------------------------------------------------

    private void SaveHomePlanetoidPlants()
    {
        if (Universe.Instance != null)
        {
            foreach (var homePlanetoid in Universe.Instance.AllHomePlanetoids)
            {
                var plantSaveDataList = new List<PlantSaveData>();

                foreach (var plant in homePlanetoid.Plants)
                {
                    plantSaveDataList.Add(new PlantSaveData(plant));
                }

                SaveGameData.UpdateHomePlanetoidPlants(homePlanetoid.PlanetoidType, plantSaveDataList);
            }
        }
    }

    // -------------------------------------------------------------------------------

    private void LoadHomePlanetoidPlants()
    {
        if (SaveGameData.HomePlanetoidPlants != null)
        {
            foreach (var homePlanetoidPlantListPair in SaveGameData.HomePlanetoidPlants)
            {
                var homePlanetoid = Universe.Instance.GetHomePlanetoid(homePlanetoidPlantListPair.PlanetoidType);

                foreach (var plantSaveData in homePlanetoidPlantListPair.PlantSaveDataList)
                {
                    var spawnedPlant = PlantManager.Instance.SpawnPlant(plantSaveData.PlantType, plantSaveData.Position, 
                        homePlanetoid, plantSaveData.GroundNormal, false);

                    spawnedPlant.NormalisedGrowth = plantSaveData.NormalisedGrowth;
                    homePlanetoid.AddPlant(spawnedPlant);
                }
            }
        }
        else
        {
            SaveGameData.HomePlanetoidPlants = new List<PlanetoidAndPlantSaveDataList>();
        }
    }

    // -------------------------------------------------------------------------------
}