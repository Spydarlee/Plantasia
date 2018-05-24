using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeedShip : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    [Header("Interior")]
    public SeedShipInterior     Interior = null;
    public Transform            WindowTransform = null;
    public Transform            EnterExteriorCamTarget = null;

    [Header("Seeds")]
    public int                  NumSeedsPerPlantTypeToSpawn = 3;
    public float                SeedSpawnRadius = 4.0f;
    public GameObject           SeedPrefab = null;

    [Header("Other")]
    public float                InputDeltaForTakeOff = 10.0f;
    public float                InputDeltaForLanding = -10.0f;
    public Transform            ViewportTopLeft = null;
    public Transform            ViewportBottomRight = null;

    // -------------------------------------------------------------------------------

    private CameraController                    mCameraController = null;
    private Seed                                mCurrentSeed = null;
    private Dictionary<PlantTypes, List<Seed>>  mSeeds = new Dictionary<PlantTypes, List<Seed>>();

    // -------------------------------------------------------------------------------

    public Seed                 CurrentSeed { get { return mCurrentSeed; } set { UpdateCurrentSeed(value); } }
    public Planetoid            PlanetoidOwner { get; set; }

    // -------------------------------------------------------------------------------

    void Start()
    {
        mCameraController = CameraController.Instance;
        Interior.Owner = this;
        PlanetoidOwner = GameManager.Instance.Universe.CurrentHomePlanetoid;
        Planetoid.OnSeedPlanted += OnSeedPlanted;

        // Instantiate a load of seeds to get us started
        if (Application.isEditor)
        {
            foreach (PlantTypes plantType in System.Enum.GetValues(typeof(PlantTypes)))
            {
                for (int i = 0; i < NumSeedsPerPlantTypeToSpawn; i++)
                {
                    SpawnSeed(plantType);
                }
            }
        }

        if (SaveGameManager.Instance.IsLoaded)
        {
            OnSaveGameLoaded(SaveGameManager.Instance.SaveGameData);
        }
        else
        {
            SaveGameManager.OnSaveGameLoaded += OnSaveGameLoaded;
        }
    }

    // -------------------------------------------------------------------------------

    private void OnSaveGameLoaded(GameData gameData)
    {
        foreach (var seedSaveData in gameData.SeedCollection)
        {
            for (int i = 0; i < seedSaveData.Count; i++)
            {
                SpawnSeed(seedSaveData.PlantType);
            }
        }
    }

    // -------------------------------------------------------------------------------

    private void SpawnSeed(PlantTypes plantType)
    {
        var spawnPosition = Interior.SeedSpawnPos.position;
        spawnPosition.x += Random.Range(-SeedSpawnRadius, SeedSpawnRadius);
        spawnPosition.z += Random.Range(-SeedSpawnRadius, SeedSpawnRadius);

        var newSeed = PlantManager.Instance.SpawnSeed(plantType, spawnPosition, Interior.transform);
        newSeed.SeedShipOwner = this;
        AddNewSeed(newSeed, false);
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        // Update the interior camera to match our rotation to give a more "3D" effect
        if (Interior.InteriorCamera != null && mCameraController && WindowTransform)
        {
            Interior.InteriorCamera.transform.forward = -WindowTransform.forward;

            var euler = Interior.InteriorCamera.transform.eulerAngles;
            euler.y = -euler.y;
            Interior.InteriorCamera.transform.eulerAngles = euler;
        }
    }

    // -------------------------------------------------------------------------------

    public void ExitInterior()
    {
        GameManager.Instance.ChangeGameplayState(GameplayStates.Planetoid);
    }

    // -------------------------------------------------------------------------------

    public void SetAttachedToPlanetoid(bool attached)
    {
        if (attached)
        {
            transform.SetParent(PlanetoidOwner.SeedShipLandingPosition);
        }
        else
        {
            transform.SetParent(null);
        }
    }

    // -------------------------------------------------------------------------------

    public void AddNewSeed(Seed newSeed, bool updateSaveGame = true)
    {
        newSeed.transform.localScale = Vector3.one;
        newSeed.transform.position = Interior.SeedEntryExitPos.position;
        newSeed.transform.SetParent(Interior.transform);
        newSeed.SetMode(Seed.Mode.InTheShip);

        if (mSeeds.ContainsKey(newSeed.PlantType))
        {
            mSeeds[newSeed.PlantType].Add(newSeed);
        }
        else
        {
            var seedList = new List<Seed>();
            seedList.Add(newSeed);
            mSeeds.Add(newSeed.PlantType, seedList);
        }

        UIManager.Instance.CurrentSeedUI.Initialise(newSeed.PlantType, GetSeedCount(newSeed.PlantType));

        if (updateSaveGame)
        {
            SaveGameManager.Instance.SaveSeedCollection(this);
        }        
    }

    // -------------------------------------------------------------------------------

    public void OnSeedPlanted(Seed seed, Plant plant)
    {
        if (seed == CurrentSeed)
        {
            mSeeds[CurrentSeed.PlantType].Remove(CurrentSeed);
            Seed newCurrentSeed = null;

            // Do we have another seed of the same type that we can set as the current seed?
            if (mSeeds[CurrentSeed.PlantType].Count > 0)
            {
                newCurrentSeed = mSeeds[CurrentSeed.PlantType][0];
            }

            // KB TODO: Return to pool
            GameObject.Destroy(CurrentSeed.gameObject);
            CurrentSeed = newCurrentSeed;

            if (CurrentSeed == null)
            {
                UIManager.Instance.CurrentSeedUI.Hide();
            }
        }
    }

    // -------------------------------------------------------------------------------

    public int GetSeedCount(PlantTypes plantType)
    {
        if (mSeeds.ContainsKey(plantType))
        {
            return mSeeds[plantType].Count;
        }

        return 0;
    }

    // -------------------------------------------------------------------------------

    public int GetSeedCount()
    {
        int count = 0;

        foreach (var seedList in mSeeds)
        {
            count += seedList.Value.Count;
        }

        return count;
    }

    // -------------------------------------------------------------------------------

    public void SetSeedPhysicsEnabled(bool enabled)
    {
        foreach (var seedPair in mSeeds)
        {
            foreach (var seed in seedPair.Value)
            {
                seed.Rigidbody.isKinematic = !enabled;
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void SelectCurrentSeedType(PlantTypes plantType)
    {
        if (mSeeds.ContainsKey(plantType))
        {
            UpdateCurrentSeed(mSeeds[plantType][0]);
        }
    }

    // -------------------------------------------------------------------------------

    private void UpdateCurrentSeed(Seed newSeed)
    {
        if (newSeed != null)
        {
            if (GameManager.Instance.CurrentGameplayState == GameplayStates.Planetoid)
            {
                UIManager uiManager = UIManager.Instance;

                uiManager.CurrentSeedUI.Initialise(newSeed.PlantType, mSeeds[newSeed.PlantType].Count);
                if (uiManager.SeedSelectionUI.IsEnabled)
                {
                    uiManager.CurrentSeedUI.Show();
                }                
            }
        }
        else
        {
            UIManager.Instance.CurrentSeedUI.Hide();
        }

        mCurrentSeed = newSeed;
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        var planetoidIsFlippedCorrectly = (GameManager.Instance.CurrentPlanetoid.Flipable == null || GameManager.Instance.CurrentPlanetoid.IsUpsideDown);

        if (planetoidIsFlippedCorrectly)
        {
            if (eventData.delta.y > InputDeltaForTakeOff)
            {
                if (GameManager.Instance.IsCurrentGameplayState(GameplayStates.Planetoid))
                {
                    GameManager.Instance.ChangeGameplayState(GameplayStates.ChoosingAStar);
                }
            }
            else if (eventData.delta.y < InputDeltaForLanding)
            {
                if (GameManager.Instance.IsCurrentGameplayState(GameplayStates.ChoosingAStar))
                {
                    SetAttachedToPlanetoid(false);
                    GameManager.Instance.ChangeGameplayState(GameplayStates.Planetoid);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Doesn't do anything, but required so we can get the OnPointerUp event
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        var gameManager = GameManager.Instance;
        var planetoidIsFlippedCorrectly = (gameManager.CurrentPlanetoid.Flipable == null || gameManager.CurrentPlanetoid.IsUpsideDown);

        if (eventData.dragging == false && planetoidIsFlippedCorrectly && gameManager.IsCurrentGameplayState(GameplayStates.Planetoid))
        {
            gameManager.ChangeGameplayState(GameplayStates.InsideSeedShip);
        }
    }

    // -------------------------------------------------------------------------------
}
