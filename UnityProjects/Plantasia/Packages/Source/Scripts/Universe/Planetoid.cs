using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Planetoid : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public enum PlanetoidTypes
    {
        Default,
        Snowy,
        Japanese,
        Desert,
    }

    // -------------------------------------------------------------------------------

    public PlanetoidTypes   PlanetoidType = PlanetoidTypes.Default;
    public bool             IsHomePlanetoid = false;
    public Transform        SeedShipLandingPosition = null;
    public Transform        SeedShipLaunchTarget = null;
    public AnimationCurve   RotationTweenDurationCurve = null;
    public float            OuterOrbitDistance = 18.0f;
    public float            InnerOrbitDistance = 13.0f;
    public Renderer         ModelRenderer = null;
    public Color            Color = Color.white;
    public bool             AllowColorTinting = true;
    public DistantStar      DistantStar;
    public GameObject       ProxySlotsParent = null;

    [Header("Camera Settings")]
    public float            MinCameraDistance = 10.0f;
    public float            MaxCameraDistance = 40.0f;
    public float            StartCameraDistance = 20.0f;

    // -------------------------------------------------------------------------------

    private List<Plant>     mPlants = new List<Plant>();
    private SeedShip        mSeedShip = null;
    private bool            mIsPaused = false;
    private List<Transform> mProxySlots = null;

    // -------------------------------------------------------------------------------

    public delegate void SeedPlantedAction(Seed seed, Plant plant);
    public static event SeedPlantedAction OnSeedPlanted = null;

    // -------------------------------------------------------------------------------

    public bool             IsPaused { get { return mIsPaused; } set { SetIsPaused(value); } }
    public bool             IsUpsideDown { get { return (Flipable) ? Flipable.IsUpsideDown : false; } }
    public Flipable         Flipable { get; private set; }
    public Rotatable        Rotatable { get; private set; }
    public int              PlantCount { get { return mPlants.Count; } }
    public List<Plant>      Plants { get { return mPlants; } }

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Flipable = GetComponent<Flipable>();
        Rotatable = GetComponent<Rotatable>();

        if (ModelRenderer == null)
        {
            ModelRenderer = GetComponentInChildren<Renderer>();
            SetColor(Color);
        }
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        mSeedShip = GameManager.Instance.SeedShip;

        // Randomly generate some plants for non-home planetoids!
        if (!IsHomePlanetoid && mPlants.Count == 0)
        {
            Universe universe = Universe.Instance;
            PlantManager plantManager = PlantManager.Instance;

            // Randomly spawn some plants
            int numPlantsToSpawn = Random.Range(universe.PlanetoidMinNumPlants, universe.PlanetoidMaxNumPlants);
            for (int i = 0; i < numPlantsToSpawn; i++)
            {
                var plantData = plantManager.GetRandomPlantData(PlanetoidType);

                Vector3 spawnPosition;
                Vector3 spawnNormal;

                if (PlanetoidObjectSpawnHelper.Instance.GetRandomSpawnPosOnPlanetoid(plantData.BaseRadius, out spawnPosition, out spawnNormal))
                {
                    var newPlant = plantManager.SpawnPlant(plantData.PlantType, spawnPosition, this, spawnNormal, false);

                    // Random chance that the plant will start off fully grown
                    if (Random.value >= 0.9f)
                    {
                        newPlant.SetFullyGrown();
                    }
                }
                else
                {
                    Debug.LogWarning("Couldn't find anywhere to spawn plant " + i + " of " + numPlantsToSpawn + " on " + this);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------

    protected void OnEnable()
    {
        foreach (var plant in mPlants)
        {
            plant.ReplayGrowthAnimation(2.0f);
        }

        var cameraController = CameraController.Instance;
        cameraController.MinDistance = MinCameraDistance;
        cameraController.MaxDistance = MaxCameraDistance;
        cameraController.TargetDistance = StartCameraDistance;
        PlanetoidObjectSpawnHelper.Instance.UpdateCameraLookAtTarget();
    }

    // -------------------------------------------------------------------------------

    public void AddPlant(Plant newPlant)
    {
        mPlants.Add(newPlant);

        // We save the state of plants on home planetoids, so notify the SaveGameManager now
        if (IsHomePlanetoid)
        {
            SaveGameManager.Instance.Save();
        }
    }

    // -------------------------------------------------------------------------------

    public void RemovePlant(Plant plant)
    {
        if (mPlants.Contains(plant))
        {
            mPlants.Remove(plant);

            if (IsHomePlanetoid)
            {
                SaveGameManager.Instance.Save();
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void ResetRotation(System.Action onCompleteCallback = null)
    {
        var targetrotation = Vector3.zero;
        var rotationDelta = 0.0f;

        if (IsUpsideDown)
        {
            targetrotation = transform.eulerAngles;
            targetrotation.y = 180.0f;

            rotationDelta = Mathf.Abs(targetrotation.y - transform.eulerAngles.y);            
        }
        else
        {
            rotationDelta = Mathf.Abs(targetrotation.y - transform.eulerAngles.y) +
                            Mathf.Abs(targetrotation.x - transform.eulerAngles.x) +
                            Mathf.Abs(targetrotation.z - transform.eulerAngles.z);
        }

        var duration = (RotationTweenDurationCurve.keys.Length > 0) ? RotationTweenDurationCurve.Evaluate(rotationDelta) : 2.0f;
        Rotatable.PlaySFX(true);

        LeanTween.rotate(gameObject, targetrotation, duration).setEaseInOutBack().setOnComplete(() =>
        {
            Rotatable.StopSFX();
            if (onCompleteCallback != null)
            {
                onCompleteCallback.Invoke();
            }
        });
    }

    // -------------------------------------------------------------------------------

    public void SetColor(Color newTintColor)
    {
        if (ModelRenderer != null && AllowColorTinting)
        {
            Color = newTintColor;
            ModelRenderer.material.color = newTintColor;
        }
    }

    // -------------------------------------------------------------------------------

    public void SetupProxies(ref List<PlanetoidProxy> proxies)
    {
        if (mProxySlots == null && ProxySlotsParent != null)
        {
            mProxySlots = new List<Transform>();
            var proxySlots = ProxySlotsParent.GetComponentsInChildren<Transform>(true);

            foreach (Transform proxySlot in proxySlots)
            {
                if (proxySlot != ProxySlotsParent.transform)
                {
                    mProxySlots.Add(proxySlot);
                }                
            }
        }

        int proxySlotIndex = 0;
        for (int i = 0; i < proxies.Count; i++)
        {
            var proxy = proxies[i];
            var isProxyUnlocked = SaveGameManager.Instance.SaveGameData.IsHomePlanetoidTypeUnlocked(proxy.Planetoid.PlanetoidType);

            // Don't show the proxy for ourself or locked planetoid types!
            if (proxy.Planetoid == this || !isProxyUnlocked)
            {
                proxy.gameObject.SetActive(false);
            }
            else
            {
                proxy.gameObject.SetActive(true);
                proxy.SetActive(true);

                proxy.transform.SetParent(mProxySlots[proxySlotIndex++]);
                proxy.transform.localPosition = Vector3.zero;
                proxy.transform.localRotation = Quaternion.identity;
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
        if (!eventData.dragging && mSeedShip.CurrentSeed != null && !IsUpsideDown && !IsPaused)
        {
            var position = eventData.pointerCurrentRaycast.worldPosition;
            var groundNormal = eventData.pointerCurrentRaycast.worldNormal;

            if (PlantManager.Instance.CheckCanSpawnPlant(mSeedShip.CurrentSeed.PlantType, position, groundNormal, out groundNormal))
            {
                var newPlant = PlantManager.Instance.SpawnPlant(mSeedShip.CurrentSeed.PlantType, position, this, groundNormal, true);
                AddPlant(newPlant);

                if (OnSeedPlanted != null)
                {
                    OnSeedPlanted(mSeedShip.CurrentSeed, newPlant);
                }
            } 
        }
    }

    // -------------------------------------------------------------------------------

    private void SetIsPaused(bool isPaused)
    {
        if (isPaused != mIsPaused)
        {
            mIsPaused = isPaused;
            Rotatable.enabled = !mIsPaused;
        }
    }

    // -------------------------------------------------------------------------------
}
