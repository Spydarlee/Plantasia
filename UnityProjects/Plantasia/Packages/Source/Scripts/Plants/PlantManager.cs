using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public static PlantManager Instance = null;

    // -------------------------------------------------------------------------------

    public List<PlantData>              PlantDataList = new List<PlantData>();
    public GameObject                   SeedPrefab = null;

    [Header("Dirt Decals")]
    public GameObject                   DirtDecalPrefab = null;
    public float                        DirtDecalOffset = 0.1f;
    public Color                        DirtDecalUnwateredColor = Color.gray;
    public Color                        DirtDecalWateredColor = Color.white;

    // -------------------------------------------------------------------------------

    private Dictionary<PlantTypes, PlantData>   mPlantDataDict = new Dictionary<PlantTypes, PlantData>();
    private int                                 mPlantLayerId = 0;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        Instance = this;

        foreach (var plantData in PlantDataList)
        {
            mPlantDataDict.Add(plantData.PlantType, plantData);
        }

        mPlantLayerId = LayerMask.NameToLayer("Plants");
    }

    // -------------------------------------------------------------------------------

    public PlantData GetPlantData(PlantTypes plantType)
    {
        if (mPlantDataDict.ContainsKey(plantType))
        {
            return mPlantDataDict[plantType];
        }

        Debug.LogError("Couldn't find PlantData for: " + plantType.ToString() + ". Returning default plant data");
        return mPlantDataDict[0];
	}

    // -------------------------------------------------------------------------------

    public PlantData GetRandomPlantData()
    {
        int numPlants = PlantDataList.Count;
        return PlantDataList[Random.Range(0, numPlants)];
    }

    // -------------------------------------------------------------------------------

    public PlantData GetRandomPlantData(Planetoid.PlanetoidTypes planetoidType)
    {
        int numPlants = PlantDataList.Count;
        PlantData plantData = null;

        while (plantData == null)
        {
            plantData = PlantDataList[Random.Range(0, numPlants)];
            if (!plantData.CanGrowOnPlanetoid(planetoidType))
            {
                plantData = null;
            }
        }

        return plantData;
    }

    // -------------------------------------------------------------------------------

    public bool CheckCanSpawnPlant(PlantTypes plantType, Vector3 position, Vector3 groundNormal, out Vector3 averageGroundNormal)
    {
        var radius = mPlantDataDict[plantType].BaseRadius;
        return PlanetoidObjectSpawnHelper.Instance.CanSpawnObject(radius, position, groundNormal, out averageGroundNormal);
    }

    // -------------------------------------------------------------------------------

    public Plant SpawnPlant(PlantTypes plantType, Vector3 position, Planetoid planetoidOwner, Vector3 groundNormal, bool spawnedByPlayer)
    {
        var plantData = mPlantDataDict[plantType];
        var newPlant = GameObjectPooler.Instance.GetPooledInstance(plantData.ModelPrefab, position, Quaternion.identity, planetoidOwner.transform);
        newPlant.layer = mPlantLayerId;

        // Point the animator to the correct controller!
        var animator = newPlant.GetComponent<Animator>();
        animator.runtimeAnimatorController = plantData.AnimatorOverrideController;

        // Create a dirt decal
        var dirtDecalPosition = newPlant.transform.position + (newPlant.transform.up * DirtDecalOffset);
        var dirtDecal = GameObjectPooler.Instance.GetPooledInstance(DirtDecalPrefab, newPlant.transform);
        dirtDecal.transform.position = dirtDecalPosition;

        var baseCollider = newPlant.GetComponent<SphereCollider>();
        Plant plantComp = newPlant.GetComponent<Plant>();
        Rigidbody rigidBody = newPlant.GetComponent<Rigidbody>();

        // If this is a newly created Plant object, we'll need to add some components!
        if (baseCollider == null)
        {
            baseCollider = newPlant.AddComponent<SphereCollider>();         // Add a sphere collider which sits at the base of the plant
            plantComp = newPlant.AddComponent<Plant>();
            plantComp.MyAudioSource = newPlant.AddComponent<AudioSource>(); // Add an AudioSource for sound effects
            rigidBody = newPlant.AddComponent<Rigidbody>();
        }

        // Initialise our various components
        baseCollider.radius = plantData.BaseRadius;
        plantComp.OnSpawned(groundNormal, plantData, dirtDecal, planetoidOwner, spawnedByPlayer);
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        plantComp.MyAudioSource.playOnAwake = false;
        plantComp.MyAudioSource.loop = false;

        return plantComp;
    }

    // -------------------------------------------------------------------------------

    public Seed SpawnSeed(PlantTypes plantType, Vector3 position, Transform parent)
    {
        var newSeed = GameObject.Instantiate(SeedPrefab, position, Quaternion.identity, parent);
        var newSeedComp = newSeed.GetComponent<Seed>();

        newSeedComp.Initialise(GetPlantData(plantType));
        return newSeedComp;
    }

    // -------------------------------------------------------------------------------
}
