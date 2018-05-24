using UnityEngine;
using UnityEngine.EventSystems;

public class Seed : MonoBehaviour, IPointerDownHandler
{
    // -------------------------------------------------------------------------------

    public enum Mode
    {
        InTheShip,
        BeingDisplayed,
        JustBeenHarvested,
        WaitingForCollection,
        BeingCollected,
    }

    // -------------------------------------------------------------------------------

    public PlantTypes       PlantType = PlantTypes.Snowdrop;
    public Rigidbody        Rigidbody = null;
    public float            OnClickForce = 10.0f;
    public float            RandomForceCooldown = 1.0f;
    public Vector3          ScaleWhenSelected = new Vector3(0.5f, 0.5f, 0.5f);
    public AudioSource      AudioSource = null;

    [Header("Just Harvested")]
    public AnimationCurve   JustHarvestedAnimCurve = new AnimationCurve();
    public float            JustHarvestedAnimDuration = 1.5f;

    [Header("Waiting For Collection")]
    public float            WaitingForCollectionHeight = 2f;
    public float            BounceOffset = 0.5f;
    public float            BounceSpeed = 5.0f;

    // -------------------------------------------------------------------------------

    private float           mTimeSinceRandomForce = 1000.0f;
    private Mode            mMode = Mode.InTheShip;
    private float           mElapsedStateTime = 0.0f;

    // -------------------------------------------------------------------------------

    public SeedShip         SeedShipOwner { get; set; }

    // -------------------------------------------------------------------------------

    public delegate void OnSeedCollectedHandler();
    public OnSeedCollectedHandler OnSeedCollected;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        if (AudioSource == null)
        {
            AudioSource = GetComponent<AudioSource>();
        }
    }

    // -------------------------------------------------------------------------------

    public void Initialise(PlantData plantData)
    {
        PlantType = plantData.PlantType;

        // Instantiate the plant mesh that represents us
        /*var meshGameObject = GameObject.Instantiate(plantData.ModelPrefab, transform);

        if (plantData.SeedTransform != null)
        {
            meshGameObject.transform.localScale = plantData.SeedTransform.localScale;
            meshGameObject.transform.localPosition = plantData.SeedTransform.localPosition;
            meshGameObject.transform.localRotation = plantData.SeedTransform.localRotation;
        }

        mRenderer = meshGameObject.GetComponentInChildren<Renderer>();*/
    }

    // -------------------------------------------------------------------------------

    public void SetMode(Mode mode)
    {
        mMode = mode;
        mElapsedStateTime = 0.0f;

        switch (mode)
        {
            case Mode.InTheShip:
                {
                    Rigidbody.isKinematic = false;
                }
                break;

            case Mode.BeingDisplayed:
                {
                    transform.rotation = Quaternion.identity;
                    Rigidbody.isKinematic = true;
                }
                break;

            case Mode.JustBeenHarvested:
                {
                    LeanTween.scale(gameObject, transform.localScale, 0.5f);
                    transform.localScale = Vector3.zero;
                    Rigidbody.isKinematic = true;
                }
                break;

            default: break;
        }
    }

    // -------------------------------------------------------------------------------
     
    private void FixedUpdate()
    {
        switch (mMode)
        {
            case Mode.BeingDisplayed:
                {
                    transform.Rotate(transform.up);
                }
                break;

            case Mode.JustBeenHarvested:
                {
                    if (mElapsedStateTime < JustHarvestedAnimDuration)
                    {
                        var t = (mElapsedStateTime / JustHarvestedAnimDuration);
                        var heightOffset = JustHarvestedAnimCurve.Evaluate(t) * WaitingForCollectionHeight;
                        transform.position = transform.parent.position + (transform.up * heightOffset);
                    }
                    else
                    {
                        SetMode(Mode.WaitingForCollection);
                    }
                }
                break;

            case Mode.WaitingForCollection:
                {
                    var targetPosition = transform.parent.position;
                    targetPosition += transform.up * (WaitingForCollectionHeight + Mathf.Sin(mElapsedStateTime * BounceSpeed) * BounceOffset);
                    transform.position = targetPosition;
                    transform.Rotate(Vector3.up, Space.Self);
                }
                break;

            case Mode.InTheShip:
            case Mode.BeingCollected:
            default: break;
        }

        mTimeSinceRandomForce += Time.deltaTime;
        mElapsedStateTime += Time.deltaTime;
    }

    // -------------------------------------------------------------------------------

    public void ApplyRandomForce()
    {
        if (!Rigidbody.isKinematic && mTimeSinceRandomForce >= RandomForceCooldown)
        {
            Vector3 force = Vector3.up;
            force.x = Random.Range(-1.0f, 1.0f);
            force.z = Random.Range(-1.0f, 1.0f);

            Rigidbody.AddForce(force.normalized * OnClickForce, ForceMode.Impulse);
            mTimeSinceRandomForce = 0.0f;
        }
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mMode == Mode.InTheShip && Input.touchCount < 2)
        {
            ApplyRandomForce();
        }
        else if (mMode == Mode.WaitingForCollection)
        {
            var gameManager = GameManager.Instance;
            var gameObjectPooler = GameObjectPooler.Instance;

            var particleFX = gameObjectPooler.GetPooledInstance(gameManager.SeedCollectParticleEffect, transform.position, Quaternion.identity, null);
            AudioManager.Instance.PlayRandomSfx(AudioSource, 0.0f, "CollectSeed", "CollectSeed2");
            SetMode(Mode.BeingCollected);

            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setOnComplete(() => 
            {
                gameManager.SeedShip.AddNewSeed(this);
                gameObjectPooler.ReturnToPool(particleFX);
                SetMode(Mode.InTheShip);

                if (OnSeedCollected != null)
                {
                    OnSeedCollected.Invoke();
                }
            });
        }
    }

    // -------------------------------------------------------------------------------
}