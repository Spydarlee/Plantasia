using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Flipable : MonoBehaviour, IDragHandler
{
    // -------------------------------------------------------------------------------

    public float        FlipDuration = 1.0f;
    public float        InputDeltaForFlip = 50.0f;
    public AudioSource  AudioSource = null;

    // -------------------------------------------------------------------------------

    private bool    mIsUpsideDown = false;
    private bool    mIsFlipping = false;

    // -------------------------------------------------------------------------------

    public delegate void OnFlipStartedHandler(bool willBeUpsideDown);
    public delegate void OnFlipCompletedHandler(bool isUpsideDown);

    public OnFlipStartedHandler OnFlipStartedEvent;
    public OnFlipCompletedHandler OnFlipCompletedEvent;

    public bool IsUpsideDown    { get { return mIsUpsideDown; } }
    public bool IsFlipping      { get { return mIsFlipping; } }

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (AudioSource == null)
        {
            AudioSource = AudioManager.CreateAudioSource(gameObject);
        }
    }

    // -------------------------------------------------------------------------------

    public void Flip(bool flipForwards)
    {
        if (!mIsFlipping && enabled)
        {
            mIsUpsideDown = !mIsUpsideDown;
            mIsFlipping = true;

            LeanTween.delayedCall(FlipDuration * 0.25f, () => { AudioManager.Instance.PlaySfx(AudioSource, "Flip"); } );

            // Send out an event to let any listeners know we've started flipping
            if (OnFlipStartedEvent != null)
            {
                OnFlipStartedEvent(mIsUpsideDown);
            }            

            var rotationAmount = (flipForwards) ? 180.0f : -180.0f;
            LeanTween.rotateAround(gameObject, Vector3.right, rotationAmount, FlipDuration).setEaseInOutBack().setOnComplete(() =>
            {
                // When the tween is complete we consider ourselves "flipped" - let the world know!
                mIsFlipping = false;

                if (OnFlipCompletedEvent != null)
                {
                    OnFlipCompletedEvent(mIsUpsideDown);
                }                
            });
        }
    }

    // -------------------------------------------------------------------------------

    public void SetUpsideDown(bool upsideDown)
    {
        mIsFlipping = false;
        mIsUpsideDown = upsideDown;
        transform.rotation = Quaternion.identity;

        if (upsideDown)
        {
            transform.Rotate(Vector3.right, 180.0f);
        }
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2)
        {
            var absDeltaX = Mathf.Abs(eventData.delta.x);
            var absDeltaY = Mathf.Abs(eventData.delta.y);

            // Only try and flip if it seems like the player is actually trying
            // to do and isn't just panning around (i.e. y delta is clearly bigger)
            if (absDeltaY > absDeltaX)
            {
                if (Mathf.Abs(eventData.delta.y) >= InputDeltaForFlip)
                {
                    Flip(eventData.delta.y > 0.0f);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
}
