using UnityEngine;
using UnityEngine.EventSystems;

public class Cloud : Orbitable, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public ParticleSystem   RainParticleSystem = null;
    public Rain             Rain = null;
    public float            LifeTime = 15.0f;
    public float            ColliderDelay = 1.0f;

    // -------------------------------------------------------------------------------

    private float           mElapsedLifeTime = 0.0f;
    private bool            mIsDespawning = false;

    // -------------------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();

        if (RainParticleSystem == null)
        {
            RainParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        SetRainEnabled(false);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------------

    protected override void Update()
    {
        base.Update();

        mElapsedLifeTime += Time.deltaTime;
        if (!mIsDespawning && mElapsedLifeTime >= LifeTime)
        {
            Despawn();
        }
    }

    // -------------------------------------------------------------------------------

    public void Spawn(float orbitDistance)
    {
        OrbitDistance = orbitDistance;
        gameObject.SetActive(true);

        // Pick a random spawn position around the current planetoid's orbit
        var spawnPosition = mCenterOfTheUniverse;
        spawnPosition += new Vector3(Random.value, Random.value, 0.0f).normalized * OrbitDistance;
        transform.position = spawnPosition;

        // Make sure the rain collider can reach the planetoid's surface!
        var rainColliderSize = Rain.RainTriggerCollider.size;
        rainColliderSize.z = orbitDistance;
        Rain.RainTriggerCollider.size = rainColliderSize;

        var rainColliderCenter = Rain.RainTriggerCollider.center;
        rainColliderCenter.z = orbitDistance * 0.5f;
        Rain.RainTriggerCollider.center = rainColliderCenter;

        // Scale up from zero to cloud hero
        LeanTween.scale(gameObject, Vector3.one, 2.0f);
        mElapsedLifeTime = 0.0f;
    }

    // -------------------------------------------------------------------------------

    public void Despawn()
    {
        mIsDespawning = true;
        SetRainEnabled(false);

        LeanTween.scale(gameObject, Vector3.zero, 2.0f).setOnComplete(() =>
       {
           gameObject.SetActive(false);
           mIsDespawning = false;
           Universe.Instance.NotifyCloudDespawned(this);
       });
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Anytime the player interacts with the cloud we reset the lifetime counter
        mElapsedLifeTime = 0.0f;
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        // Toggle the rain
        if (!eventData.dragging && Rain != null && !mIsDespawning)
        {
            SetRainEnabled(!Rain.CollisionEnabled);
        }
    }

    // -------------------------------------------------------------------------------

    private void SetRainEnabled(bool enabled)
    {
        var emission = RainParticleSystem.emission;
        emission.enabled = enabled;
        Rain.CollisionEnabled = enabled;
    }

    // -------------------------------------------------------------------------------
}
