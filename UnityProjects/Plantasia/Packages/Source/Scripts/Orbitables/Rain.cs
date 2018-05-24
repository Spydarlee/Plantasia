using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public BoxCollider  RainTriggerCollider = null;
    public float        ColliderToggleDelay = 1.5f;
    public AudioSource  RainingAudioSource = null;

    // -------------------------------------------------------------------------------

    public bool CollisionEnabled { get { return mEnabled; } set { SetEnabled(value); } }

    // -------------------------------------------------------------------------------

    private List<Plant>     mPlants = new List<Plant>();
    private bool            mEnabled = false;
    private float           mColliderToggleDelay = 0.0f;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (RainingAudioSource == null)
        {
            RainingAudioSource = GetComponent<AudioSource>();
        }
    }

    // -------------------------------------------------------------------------------

    private void SetEnabled(bool enabled)
    {
        mEnabled = enabled;
        mColliderToggleDelay = ColliderToggleDelay;

        if (RainingAudioSource != null)
        {
            if (enabled)
            {
                AudioManager.Instance.FadeAudioSourceIn(RainingAudioSource, 1.0f);
            }
            else
            {
                AudioManager.Instance.FadeAudioSourceOut(RainingAudioSource, 1.0f);
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        if (mColliderToggleDelay > 0.0f)
        {
            mColliderToggleDelay -= Time.deltaTime;
            if (mColliderToggleDelay <= 0.0f)
            {
                RainTriggerCollider.enabled = mEnabled;

                // Tell any plants if we've stopped raining on them!
                if (!mEnabled)
                {
                    foreach (var plant in mPlants)
                    {
                        plant.IsBeingRainedOn = false;
                    }

                    mPlants.Clear();
                }
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void OnTriggerEnter(Collider other)
    {
        var plant = other.GetComponent<Plant>();
        if (plant != null)
        {
            plant.IsBeingRainedOn = true;
            mPlants.Add(plant);
        }
    }

    // -------------------------------------------------------------------------------

    public void OnTriggerExit(Collider other)
    {
        var plant = other.GetComponent<Plant>();
        if (plant != null)
        {
            plant.IsBeingRainedOn = false;
            mPlants.Remove(plant);
        }
    }

    // -------------------------------------------------------------------------------
}
