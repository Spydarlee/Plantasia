using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static Universe      Instance;

    // -------------------------------------------------------------------------------

    public UniverseSphere       UniverseSphere = null;
    public Planetoid            CurrentHomePlanetoid = null;
    public List<Planetoid>      AllHomePlanetoids = new List<Planetoid>();
    public Planetoid            CurrentPlanetoid = null;
    public Sun                  TheSun = null;
    public Orbitable            TheMoon = null;
    public List<GameObject>     DistantStarPrefabs = new List<GameObject>();    
    public int                  NumDistantStars = 1;
    public float                MinSqrDistanceBetweenStars = 100.0f;
    public Transform            ShipTakeOffStart = null;
    public Transform            ShipTakeOffEnd = null;

    [Header("Clouds")]
    public GameObject           CloudPrefab = null;
    public float                TimeBetweenCloudSpawns = 20.0f;
    public bool                 CloudSpawningEnabled = true;

    [Header("Planetoids")]
    public List<GameObject>     PlanetoidPrefabs = new List<GameObject>();
    public List<Color>          PlanetoidColors = new List<Color>();
    public List<PlanetoidProxy> PlanetoidProxies = new List<PlanetoidProxy>();
    public int                  PlanetoidMinNumPlants = 5;
    public int                  PlanetoidMaxNumPlants = 30;
    public LayerMask            PlanetoidLayerMask;

    // -------------------------------------------------------------------------------

    public bool                 IsOnHomePlanetoid { get { return CurrentPlanetoid == CurrentHomePlanetoid; } }

    // -------------------------------------------------------------------------------

    public delegate void PlanetoidChangeAction(Planetoid newPlanetoid);
    public event PlanetoidChangeAction OnPlanetoidChange = null;

    // -------------------------------------------------------------------------------

    private GameManager         mGameManager = null;
    private SeedShip            mSeedShip = null;
    private List<DistantStar>   mDistantStars = new List<DistantStar>();
    private List<Planetoid>     mPlanetoids = new List<Planetoid>();

    private List<Cloud>         mInactiveClouds = new List<Cloud>();
    private List<Cloud>         mActiveClouds = new List<Cloud>();
    private float               mTimeSinceLastCloudSpawn = 0.0f;

    private Dictionary<Planetoid.PlanetoidTypes, GameObject> mDistantStarPrefabs = new Dictionary<Planetoid.PlanetoidTypes, GameObject>();

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        foreach (var distantStarPrefab in DistantStarPrefabs)
        {
            var distantStar = distantStarPrefab.GetComponent<DistantStar>();
            mDistantStarPrefabs.Add(distantStar.PlanetoidType, distantStarPrefab);
        }
    }

    // -------------------------------------------------------------------------------

    void Start()
    {
        mGameManager = GameManager.Instance;
        CurrentPlanetoid = CurrentHomePlanetoid;
        mSeedShip = mGameManager.SeedShip;
        TheSun.OrbitDistance = CurrentHomePlanetoid.OuterOrbitDistance;
        TheMoon.OrbitDistance = CurrentHomePlanetoid.OuterOrbitDistance;

        if (SaveGameManager.Instance.IsLoaded)
        {
            OnSaveGameLoaded(SaveGameManager.Instance.SaveGameData);
        }
        else
        {
            SaveGameManager.OnSaveGameLoaded += OnSaveGameLoaded;
        }        

        // Setup the starz for our home planetoids
        foreach (var homePlanetoid in AllHomePlanetoids)
        {
            homePlanetoid.DistantStar.Universe = this;
            homePlanetoid.DistantStar.Planetoid = homePlanetoid;
            mDistantStars.Add(homePlanetoid.DistantStar);
            homePlanetoid.DistantStar.Hide(true);
            homePlanetoid.DistantStar.transform.parent = UniverseSphere.transform;
        }

        // Attach the seed ship to our home planet
        mGameManager = GameManager.Instance;
        mSeedShip.PlanetoidOwner = CurrentHomePlanetoid;
        mSeedShip.SetAttachedToPlanetoid(true);
        mSeedShip.transform.localPosition = Vector3.zero;
        mSeedShip.transform.localRotation = Quaternion.identity;

        UpdateShiptTakeOffPoints();
        
        // Disable the home planetoid while we're spawning other planetoids because
        // we need to cast rays at the center of the universe and we don't want to hit
        // the home planetoid or the ship by mistake!
        CurrentHomePlanetoid.gameObject.SetActive(false);
        mSeedShip.gameObject.SetActive(false);

        // We want the universe sphere to be active so that new distant stars
        // get their Awake called for correct initialisation
        UniverseSphere.gameObject.SetActive(true);
        UniverseSphere.Hide();

        // Spawn a number of distant stars and planetoids to explore!
        for (int i = 0; i < NumDistantStars; i++)
        {
            var randomColor = PlanetoidColors[Random.Range(0, PlanetoidColors.Count)];
            var newPlanetoid = GenerateNewPlanetoid(randomColor);

            // Spawn a star for this new planetoid
            var distantStarSpawnPos = GetRandomDistantStarPosition();
            var newDistantStar = GameObject.Instantiate(GetDistantStarPrefab(newPlanetoid.PlanetoidType), distantStarSpawnPos, Quaternion.identity, UniverseSphere.transform).GetComponent<DistantStar>();
            newDistantStar.Universe = this;
            newDistantStar.SetTintColor(newPlanetoid.AllowColorTinting ? randomColor : Color.white);
            mDistantStars.Add(newDistantStar);

            // Make sure the star and planet know about each other
            newDistantStar.Planetoid = newPlanetoid;
            newPlanetoid.DistantStar = newDistantStar;

            // Hide our new star and planetoid
            newDistantStar.Hide(true);
            newPlanetoid.gameObject.SetActive(false);
        }

        // Safe to turn these back on (or off) now!
        CurrentHomePlanetoid.gameObject.SetActive(true);
        mSeedShip.gameObject.SetActive(true);
        UniverseSphere.gameObject.SetActive(false);

        // Create some clouds that we can keep using and re-using
        for (int i = 0; i < 3; i++)
        {
            var newCloud = GameObject.Instantiate(CloudPrefab, transform);
            mInactiveClouds.Add(newCloud.GetComponent<Cloud>());
        }

        mTimeSinceLastCloudSpawn = (TimeBetweenCloudSpawns - 5.0f);
    }

    // -------------------------------------------------------------------------------

    private void OnSaveGameLoaded(GameData gameData)
    {
        CurrentHomePlanetoid.SetupProxies(ref PlanetoidProxies);
    }

    // -------------------------------------------------------------------------------

    private void Update()
    {
        // TODO: Dynamically update timeline references to use palnet-specific transforms
        // instead of keeping this constantly up to date? Hmm
        UpdateShiptTakeOffPoints();

        if (GameManager.Instance.CurrentGameplayState == GameplayStates.Planetoid && CloudSpawningEnabled)
        {
            mTimeSinceLastCloudSpawn += Time.deltaTime;
            if (mTimeSinceLastCloudSpawn >= TimeBetweenCloudSpawns && mInactiveClouds.Count > 0)
            {
                var cloud = mInactiveClouds[0];
                mInactiveClouds.Remove(cloud);
                mActiveClouds.Add(cloud);
                cloud.Spawn(mGameManager.CurrentPlanetoid.InnerOrbitDistance);
                mTimeSinceLastCloudSpawn = 0.0f;
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void ChooseNewPlanetoid(Planetoid newPlanetoid)
    {
        if (newPlanetoid != mGameManager.CurrentPlanetoid)
        {
            var currentPlanetoid = mGameManager.CurrentPlanetoid;
            currentPlanetoid.gameObject.SetActive(false);

            // Make sure flipable planets are upside down before we land!
            if (newPlanetoid.Flipable != null)
            {
                newPlanetoid.Flipable.SetUpsideDown(true);
            }

            if (newPlanetoid.IsHomePlanetoid)
            {
                CurrentHomePlanetoid = newPlanetoid;
                CurrentHomePlanetoid.SetupProxies(ref PlanetoidProxies);
            }

            mGameManager.CurrentPlanetoid = newPlanetoid;
            newPlanetoid.gameObject.SetActive(false); // Don't turn it until we're ready to land!

            UpdateShiptTakeOffPoints();
            TheSun.OrbitDistance = newPlanetoid.OuterOrbitDistance;
            TheMoon.OrbitDistance = newPlanetoid.OuterOrbitDistance;
            mSeedShip.SetAttachedToPlanetoid(false);

            // Let any interested parties know about the change
            if (OnPlanetoidChange != null)
            {
                OnPlanetoidChange.Invoke(newPlanetoid);
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void NotifyCloudDespawned(Cloud cloud)
    {
        mInactiveClouds.Add(cloud);
        mActiveClouds.Remove(cloud);
    }

    // -------------------------------------------------------------------------------

    public void DespawnAllClouds()
    {
        foreach (var cloud in mActiveClouds)
        {
            cloud.Despawn();
        }

        mActiveClouds.Clear();
        mTimeSinceLastCloudSpawn = 0.0f;
    }

    // -------------------------------------------------------------------------------

    public void ShowDistantStars()
    {
        bool currentlyOnHomePlanetoid = (CurrentPlanetoid == CurrentHomePlanetoid);

        foreach (var distantStar in mDistantStars)
        {
            bool shouldShowHomePlanetoid = !currentlyOnHomePlanetoid;
            shouldShowHomePlanetoid &= distantStar.Planetoid == CurrentHomePlanetoid;

            // Don't show the current planetoid, we're supposed to be there!
            // Also only show the current home planetoid if we are away from home
            if ((distantStar.Planetoid != CurrentPlanetoid) &&
                (!distantStar.Planetoid.IsHomePlanetoid || shouldShowHomePlanetoid))
            {
                distantStar.Show();
            }
        }

        UniverseSphere.gameObject.SetActive(true);
        UniverseSphere.Show();
    }

    // -------------------------------------------------------------------------------

    public void HideDistantStars()
    {
        UniverseSphere.Hide();

        foreach (var distantStar in mDistantStars)
        {
            distantStar.Hide();
        }
    }

    // -------------------------------------------------------------------------------

    public Planetoid GetHomePlanetoid(Planetoid.PlanetoidTypes planetoidType)
    {
        foreach (var homePlanetoid in AllHomePlanetoids)
        {
            if (homePlanetoid.PlanetoidType == planetoidType)
            {
                return homePlanetoid;
            }
        }

        return null;
    }

    // -------------------------------------------------------------------------------

    private void UpdateShiptTakeOffPoints()
    {
        // These transforms are used as animation targets during state transitions

        ShipTakeOffStart.position = mGameManager.CurrentPlanetoid.SeedShipLandingPosition.position;
        ShipTakeOffStart.rotation = mGameManager.CurrentPlanetoid.SeedShipLandingPosition.rotation;
        ShipTakeOffEnd.position = mGameManager.CurrentPlanetoid.SeedShipLaunchTarget.position;
        ShipTakeOffEnd.rotation = mGameManager.CurrentPlanetoid.SeedShipLaunchTarget.rotation;
    }

    // -------------------------------------------------------------------------------

    private Planetoid GenerateNewPlanetoid(Color planetoidColor)
    {
        // Pick a random new kind of planetoid to spawn, and customise it a bit
        var randomPlanetoidIndex = Random.Range(0, PlanetoidPrefabs.Count);
        var newPlanetoid = GameObject.Instantiate(PlanetoidPrefabs[randomPlanetoidIndex], Vector3.zero, Quaternion.identity, transform).GetComponent<Planetoid>();
        newPlanetoid.SetColor(planetoidColor);
        mPlanetoids.Add(newPlanetoid);

        return newPlanetoid;
    }

    // -------------------------------------------------------------------------------

    private GameObject GetDistantStarPrefab(Planetoid.PlanetoidTypes planetoidType)
    {
        GameObject distantStarPrefab = null;

        if (!mDistantStarPrefabs.TryGetValue(planetoidType, out distantStarPrefab))
        {
            Debug.LogWarning("Couldn't find Distant Star prefab for Planetoid type: " + planetoidType + ". Using Default isntead");
            distantStarPrefab = mDistantStarPrefabs[Planetoid.PlanetoidTypes.Default];
        }

        return distantStarPrefab;
    }

    // -------------------------------------------------------------------------------

    private Vector3 GetRandomDistantStarPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool foundGoodRandomPosition = false;
        int numAttempts = 0;

        while (!foundGoodRandomPosition && numAttempts < 10)
        {
            randomPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * UniverseSphere.Radius;
            bool tooCloseToStar = false;

            foreach (var distantStar in mDistantStars)
            {
                // Are we too close to this star to be a valid spawn position?
                if (Vector3.SqrMagnitude(randomPosition - distantStar.transform.position) < MinSqrDistanceBetweenStars)
                {
                    tooCloseToStar = true;
                    break;
                }
            }

            if (tooCloseToStar)
            {
                ++numAttempts;
            }
            else
            {
                foundGoodRandomPosition = true;
            }            
        }

        return randomPosition;        
    }

    // -------------------------------------------------------------------------------
}
