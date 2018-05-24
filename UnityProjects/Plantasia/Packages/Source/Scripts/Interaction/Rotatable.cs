using UnityEngine;
using UnityEngine.EventSystems;

public class Rotatable : MonoBehaviour, IDragHandler
{
    public float                InputMultiplier = 0.5f;
    public AnimationCurve       SpeedCurve = new AnimationCurve();
    public bool                 EnableAudio = true;
    public AudioSource          AudioSource = null;
    public float                MinRotationForSFX = 1.0f;

    public bool                 EnablePitch = true;
    public bool                 EnableYaw = true;
    public bool                 InvertPitch = false;
    public bool                 InvertYaw = false;

    // -------------------------------------------------------------------------------

    public bool                 DisableInput { get; set; }
    public float                TimeSinceInput { get { return mTimeSinceInput; } }

    // -------------------------------------------------------------------------------

    private float               mYaw = 0.0f;
    private float               mPitch = 0.0f;
    private float               mTimeSinceInput = 0.0f;
    private bool                mWaitForManualSFXStop = false;

    // -------------------------------------------------------------------------------

    void Start()
    {
        if (EnableAudio && AudioSource == null)
        {
            AudioSource = AudioManager.CreateAudioSource(gameObject, "Woosh", true);
        }
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        var pitchThisFrame = GetFrameRotation(mPitch);
        var yawThisFrame = GetFrameRotation(mYaw);

        transform.Rotate(pitchThisFrame, yawThisFrame, 0f, Space.World);

        mYaw = RemoveAppliedRotation(mYaw, yawThisFrame);
        mPitch = RemoveAppliedRotation(mPitch, pitchThisFrame);

        mTimeSinceInput += Time.deltaTime;

        // Play audio when the Rotatable is actively being rotated
        if (AudioSource != null)
        {
            var rotationThisFrame = Mathf.Abs(pitchThisFrame) + Mathf.Abs(yawThisFrame);
            var shouldPlaySFX = (rotationThisFrame >= MinRotationForSFX);

            if (shouldPlaySFX)
            {
                PlaySFX();
            }
            else if(!mWaitForManualSFXStop)
            {
                StopSFX();   
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (!DisableInput && Input.touchCount < 2)
        {
            Rotate(eventData.delta);
        }
    }

    // -------------------------------------------------------------------------------

    public void Rotate(Vector2 inputDelta)
    {
        if (EnableYaw)
        {
            var newYaw = (inputDelta.x * InputMultiplier);
            mYaw = (InvertYaw) ? (mYaw + newYaw) : (mYaw - newYaw);
        }

        if (EnablePitch)
        {
            var newPitch = (inputDelta.y * InputMultiplier);
            mPitch = (InvertPitch) ? (mPitch - newPitch) : (mPitch + newPitch);
        }

        mTimeSinceInput = 0.0f;
    }

    // -------------------------------------------------------------------------------

    public void PlaySFX(bool waitForManualStop = false)
    {
        if (!AudioSource.isPlaying)
        {
            mWaitForManualSFXStop = waitForManualStop;
            var audioManager = AudioManager.Instance;
            audioManager.PlaySfx(AudioSource, "Woosh");
            audioManager.FadeAudioSourceIn(AudioSource, 0.1f);
        }
    }

    // -------------------------------------------------------------------------------

    public void StopSFX()
    {
        if (AudioSource.isPlaying)
        {
            AudioManager.Instance.FadeAudioSourceOut(AudioSource, 0.6f);
            mWaitForManualSFXStop = false;
        }
    }

    // -------------------------------------------------------------------------------

    private float RemoveAppliedRotation(float rotation, float delta)
    {
        if (rotation > 0.0f)
        {
            rotation -= delta;
            rotation = Mathf.Clamp(rotation, 0.0f, Mathf.Infinity);
        }
        else
        {
            rotation -= delta;
            rotation = Mathf.Clamp(rotation, -Mathf.Infinity, 0.0f);
        }

        return rotation;
    }

    // -------------------------------------------------------------------------------

    private float GetFrameRotation(float totalRotation)
    {
        if (totalRotation != 0.0f)
        {
            var directionMultiplier = (totalRotation < 0.0f) ? -1f : 1f;
            return SpeedCurve.Evaluate(Mathf.Abs(totalRotation)) * directionMultiplier;
        }

        return 0.0f;
    }

    // -------------------------------------------------------------------------------
}
