using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public PlantTypes               PlantType = PlantTypes.Snowdrop;
    public Animator                 Animator = null;
    public float                    InitialGrowthDuration = 0.5f;
    public AudioSource              MyAudioSource = null;

    public float NormalisedGrowth
    {
        get { return mNormalisedGrowth; }
        set { SetNormalisdGrowth(value, true); }
    }

    public Seed     SeedToBeCollected { get; private set; }
    public Vector3  GroundNormal { get; private set; }

    // -------------------------------------------------------------------------------

    private SkinnedMeshRenderer     mSkinnedMeshRenderer = null;
    private Renderer                mDirtDecalRenderer = null;
    private int                     mNumGrowAnims = 1;
    private float                   mGrowthTime = 0.0f;
    private float                   mMainGrowth = 1.0f;
    private float                   mNormalisedGrowth = 0.0f;
    private int                     mNumCloudsRainingOnMe = 0;

    private int                     mWilteredShaderPopertyId = 0;
    private bool                    mBeingHarvested = false;
    private float                   mLastClickTime = 0.0f;
    private bool                    mNeedToSetFullyGrown = false;
    private bool                    mHasHadFirstUpdate = false;
    private PlantManager            mPlantManager = null;

    // Growth requirements
    public float                    WaterCharge { get; set; }
    private bool                    mGrowthRequirementsMet = false;
    private int                     mWiltedTweenId = 0;
    private float                   mWilted = 1.0f;
    private float                   mProcessClickCountdown = 0.0f;
    private bool                    mReplayingGrowth = false;
    private bool                    mRemovePlantAfterSeedHarvested = false;
    private Planetoid               mPlanetoidOwner = null;

    // -------------------------------------------------------------------------------

    public void OnSpawned(Vector3 groundNormal, PlantData plantData, GameObject dirtDecal, Planetoid planetoidOwner, bool spawnedByPlayer)
    {
        if (!Animator)
        {
            Animator = GetComponent<Animator>();
        }

        mWilteredShaderPopertyId = Shader.PropertyToID("_Wilted");
        mPlanetoidOwner = planetoidOwner;

        mPlantManager = PlantManager.Instance;
        PlantData = plantData;
        mMainGrowth = (1.0f - PlantData.InitialGrowth);
        mBeingHarvested = false;
        mNormalisedGrowth = 0.0f;
        mGrowthTime = 0.0f;

        mDirtDecalRenderer = dirtDecal.GetComponent<Renderer>();
        if (spawnedByPlayer)
        {
            LeanTween.scale(mDirtDecalRenderer.gameObject, mDirtDecalRenderer.transform.localScale, 1.0f);
            mDirtDecalRenderer.transform.localScale = Vector3.zero;
            AudioManager.Instance.PlayRandomSfx(MyAudioSource, 0f, "Drop1", "Drop2", "Drop3");
        }
        mDirtDecalRenderer.material.color = Color.Lerp(mPlantManager.DirtDecalUnwateredColor, mPlantManager.DirtDecalWateredColor, Mathf.Clamp01(WaterCharge));

        mSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        PlantType = plantData.PlantType;
        mNumGrowAnims = plantData.NumGrowthAnims;
        Animator.SetInteger("GrowIndex", Random.Range(0, mNumGrowAnims));

        transform.up = GroundNormal = groundNormal;
        mSkinnedMeshRenderer.material.SetFloat(mWilteredShaderPopertyId, mWilted);
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        mHasHadFirstUpdate = true;

        if (mBeingHarvested || mReplayingGrowth)
            return;

        if (mNeedToSetFullyGrown)
            SetFullyGrown();

        // We might need to process some input if we've been holding off
        // to see if the user was gonna double click or not!
        if (mProcessClickCountdown > 0.0f)
        {
            mProcessClickCountdown -= Time.deltaTime;
            if (mProcessClickCountdown <= 0.0f)
            {
                ProcessClick();
            }
        }

        bool wasFullyGrown = IsFullyGrown;

        // Update growth requirements!
        UpdateGrowthRequirements();

        // Groooooooooow!
        if (mGrowthTime < PlantData.GrowthDuration)
        {
            if (mNormalisedGrowth < PlantData.InitialGrowth)
            {
                mGrowthTime += Time.deltaTime;
                mNormalisedGrowth = (Mathf.Clamp01(mGrowthTime / InitialGrowthDuration)) * PlantData.InitialGrowth;
            }
            else
            {
                if (mGrowthRequirementsMet)
                {
                    mGrowthTime += Time.deltaTime;
                    mNormalisedGrowth = PlantData.InitialGrowth + (mGrowthTime / (PlantData.GrowthDuration - InitialGrowthDuration)) * mMainGrowth;
                }
            }

            SetAnimatorGrowth(mNormalisedGrowth);
            mNormalisedGrowth = Mathf.Clamp01(mNormalisedGrowth);
        }

        SetAnimatorGrowth(mNormalisedGrowth);

        if (!wasFullyGrown && IsFullyGrown)
        {
            UIManager.Instance.PlantStatusUI.NotifyPlantChanged(this);
        }
    }

    // -------------------------------------------------------------------------------

    public bool IsBeingRainedOn
    {
        get { return (mNumCloudsRainingOnMe > 0); }
        set { mNumCloudsRainingOnMe = (value) ? mNumCloudsRainingOnMe + 1 : mNumCloudsRainingOnMe - 1; }
    }

    // -------------------------------------------------------------------------------

    public bool IsFullyGrown { get { return mNormalisedGrowth >= 1.0f; } }

    // -------------------------------------------------------------------------------

    public PlantData PlantData { get; private set; }

    // -------------------------------------------------------------------------------

    public void SetFullyGrown()
    {
        if (mHasHadFirstUpdate)
        {
            mNeedToSetFullyGrown = false;

            // KB TODO: MAX OUT ALL REQUIREMENTS?

            UpdateGrowthRequirements();
            mNormalisedGrowth = 1.0f;
            SetAnimatorGrowth(mNormalisedGrowth);
            mGrowthTime = PlantData.GrowthDuration;
            UIManager.Instance.PlantStatusUI.NotifyPlantChanged(this);
        }
        else
        {
            mNeedToSetFullyGrown = true;
        }
    }

    // -------------------------------------------------------------------------------

    private void SetNormalisdGrowth(float normalisedGrowth, bool replayGrowthAnimation)
    {
        mNormalisedGrowth = normalisedGrowth;
        mGrowthTime = PlantData.GrowthDuration * normalisedGrowth;
        mWilted = (normalisedGrowth == 1.0f) ? 0.0f : mWilted;
        mSkinnedMeshRenderer.material.SetFloat(mWilteredShaderPopertyId, mWilted);

        if (replayGrowthAnimation)
        {
            ReplayGrowthAnimation(2.0f, mGrowthTime);
        }        
    }

    // -------------------------------------------------------------------------------

    public void ReplayGrowthAnimation(float maxDuration, float startingGrowthTime = 0.0f)
    {
        if (mReplayingGrowth)
            return;

        var duration = (maxDuration * mNormalisedGrowth);
        mReplayingGrowth = true;
        mGrowthTime = startingGrowthTime;

        LeanTween.value(0.0f, mNormalisedGrowth, duration).setOnUpdate((float value) =>
        {
            SetAnimatorGrowth(value);

        }).setOnComplete(() =>
        {
            mReplayingGrowth = false;
        });

        LeanTween.scale(mDirtDecalRenderer.gameObject, mDirtDecalRenderer.transform.localScale, 1.0f);
        mDirtDecalRenderer.transform.localScale = Vector3.zero;
    }

    // -------------------------------------------------------------------------------

    private void UpdateGrowthRequirements()
    {
        var growthRequirementsWereMet = mGrowthRequirementsMet;
        mGrowthRequirementsMet = true;

        // Update all growth requirements and if any fail, we can't grow
        // But update ALL of them anyway, as some will update data
        foreach (var growthRequirement in PlantData.GrowthRequirements)
        {
            if (growthRequirement.CheckRequirement(this) == false)
            {
                mGrowthRequirementsMet = false;
            }
        }

        // If we've just met (or stopped meeting) our growth requirements
        // it's time to update the plant shader's wilted value
        if (growthRequirementsWereMet != mGrowthRequirementsMet)
        {
            if (mWiltedTweenId != 0)
            {
                LeanTween.cancel(mWiltedTweenId);
            }

            var targetWiltValue = (mGrowthRequirementsMet || mNormalisedGrowth == 1.0f) ? 0f : 1f;
            if (targetWiltValue != mWilted)
            {
                mWiltedTweenId = LeanTween.value(mWilted, targetWiltValue, 2.0f).setOnUpdate((float val) =>
                {
                    mWilted = val;
                    mSkinnedMeshRenderer.material.SetFloat(mWilteredShaderPopertyId, mWilted);
                }).id;
            }
        }

        mDirtDecalRenderer.material.color = Color.Lerp(mPlantManager.DirtDecalUnwateredColor, mPlantManager.DirtDecalWateredColor, Mathf.Clamp01(WaterCharge));
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Does nothing but needed to get pointer up event
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        var isDoubleClick = (eventData.clickTime - mLastClickTime) < GameCursor.Instance.TimeBetweenDoubleClicks;

        if (isDoubleClick)
        {
            mProcessClickCountdown = 0.0f;

            // Are we being HARVESTED??
            if (eventData.dragging == false && mNormalisedGrowth >= 1.0f && !mBeingHarvested)
            {
                HarvestSeed(false);
            }
        }
        else if (!eventData.dragging)
        {
            // Delay processing this input until we're sure the user isn't trying to double click!
            mProcessClickCountdown = GameCursor.Instance.TimeBetweenDoubleClicks;
            UIManager.Instance.AboutToShowPlantStatusUI = true;
        }

        mLastClickTime = eventData.clickTime;
    }

    // -------------------------------------------------------------------------------

    public void HarvestSeed(bool removePlantAfterwards = false)
    {
        if (!mBeingHarvested)
        {
            mBeingHarvested = true;
            mRemovePlantAfterSeedHarvested = removePlantAfterwards;
            AudioManager.Instance.PlayRandomSfx(MyAudioSource, 0f, "ChillCord2", "ChillCord4");
            UIManager.Instance.PlantStatusUI.Hide();

            LeanTween.value(mNormalisedGrowth, 0.0f, 2.0f).setOnUpdate((float value) =>
            {
                mNormalisedGrowth = value;
                SetAnimatorGrowth(value);

            }).setOnComplete(() =>
            {
                WaterCharge = 0.0f;

                // Create our very own baby seed
                SeedToBeCollected = mPlantManager.SpawnSeed(PlantType, transform.position, transform);
                SeedToBeCollected.transform.up = transform.up;
                SeedToBeCollected.SetMode(Seed.Mode.JustBeenHarvested);
                SeedToBeCollected.OnSeedCollected += OnSeedCollected;
                AudioManager.Instance.PlayRandomSfx(MyAudioSource, 0f, "Pop1", "Pop2", "Pop3", "Pop4");
            });
        }
    }

    // -------------------------------------------------------------------------------

    private void ProcessClick()
    {
        UIManager.Instance.AboutToShowPlantStatusUI = false;

        if (!mBeingHarvested)
        {
            // Show summary UI for this here plant!
            PlantStatusUI.Instance.Show(this);
        }
    }

    // -------------------------------------------------------------------------------

    private void OnSeedCollected()
    {
        // Forget about that seed we produced
        SeedToBeCollected.OnSeedCollected -= OnSeedCollected;
        SeedToBeCollected = null;
        SaveGameManager.Instance.Save();

        if (mRemovePlantAfterSeedHarvested)
        {
            mPlanetoidOwner.RemovePlant(this);
            mRemovePlantAfterSeedHarvested = false;
            GameObjectPooler.Instance.ReturnToPool(gameObject);
            GameObjectPooler.Instance.ReturnToPool(mDirtDecalRenderer.gameObject);
        }
        else
        {
            // Reset the plant's growth!
            mBeingHarvested = false;
            mWilted = 1.0f;
            SetNormalisdGrowth(0.0f, false);
        }
    }

    // -------------------------------------------------------------------------------

    private void SetAnimatorGrowth(float growth)
    {
        Animator.SetFloat("Growth", growth);
        Animator.SetBool("FullyGrown", growth >= 1.0f);
    }

    // -------------------------------------------------------------------------------
}
