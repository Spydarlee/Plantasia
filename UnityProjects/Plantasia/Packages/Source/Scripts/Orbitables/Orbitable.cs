using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Orbitable : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // -------------------------------------------------------------------------------

    public float            DefaultOrbitSpeed = 10.0f;
    public float            MaxOrbitSpeed = 200.0f;
    public float            ExtraOrbitSpeedInputMultiplier = 10.0f;
    public float            MinExtraOrbitSpeed = 50.0f;
    public AnimationCurve   ExtraOrbitSpeedFalloffTimeCurve = new AnimationCurve();
    public float            TimeBeforeResetExtraOrbitSpeed = 0.5f;
    public Vector3          LookAtOffset = Vector3.zero;
    public List<Orbitable>  LinkedOribtables = new List<Orbitable>();
    public AudioSource      AudioSource = null;

    // -------------------------------------------------------------------------------

    protected Vector3   mCenterOfTheUniverse = Vector3.zero;
    private bool        mGrabbed = false;
    private float       mLastGrabInputDelta = 0.0f;
    private float       mCurrentOrbitSpeed = 1.0f;
    private float       mExtraOrbitSpeed = 0.0f;
    private float       mOrbitDirectionMultipler = 1f;
    private RaycastHit  mRayCastHit;
    protected float     mDirectionAngle = 0.0f;
    private float       mLastDragInputTime = 0.0f;
    private LTDescr     mTweenDescr = null;
    private float       mDragSFXTime = 0.0f;

    // -------------------------------------------------------------------------------

    public float    DirectionAngle { get { return mDirectionAngle; } }
    public bool     IsPaused { get; set; }
    public float    OrbitDistance { get; set; }

    // -------------------------------------------------------------------------------

    protected virtual void Start()
    {
        OrbitDistance = Mathf.Abs(transform.position.y);

        if (AudioSource == null)
        {
            AudioSource = GetComponent<AudioSource>();
        }

        mCurrentOrbitSpeed = DefaultOrbitSpeed;
    }

    // -------------------------------------------------------------------------------

    protected virtual void Update()
    {
        if (!mGrabbed && mTweenDescr == null && !IsPaused)
        {
            transform.RotateAround(mCenterOfTheUniverse, Vector3.forward, mCurrentOrbitSpeed * Time.deltaTime);
            mOrbitDirectionMultipler = 1f;
        }

        transform.LookAt(mCenterOfTheUniverse + LookAtOffset, transform.up);
        CalculateDirectionAngle();

        // Are we being manipulated by the player?
        if (mGrabbed || mExtraOrbitSpeed != 0.0f)
        {
            foreach (var linkedOrbitable in LinkedOribtables)
            {
                linkedOrbitable.FollowLinkedOrbitable(this);
            }
        }

        // Play audio when the orbitable is being dragged around
        if (AudioSource != null)
        {
            if (mLastGrabInputDelta != 0.0f && !AudioSource.isPlaying)
            {
                AudioManager.Instance.FadeAudioSourceIn(AudioSource, 0.2f);
                AudioSource.time = mDragSFXTime;
            }
            else if (AudioSource.isPlaying && mLastGrabInputDelta == 0.0f)
            {
                mDragSFXTime = AudioSource.time;
                AudioManager.Instance.FadeAudioSourceOut(AudioSource, 1.0f);
            }
        }

        mLastGrabInputDelta = 0.0f;
    }

    // -------------------------------------------------------------------------------

    public void FollowLinkedOrbitable(Orbitable linkedOrbitable)
    {
        var linkedToCenter = (mCenterOfTheUniverse - linkedOrbitable.transform.position).normalized;
        transform.position = mCenterOfTheUniverse + (linkedToCenter * OrbitDistance);
    }

    // -------------------------------------------------------------------------------

    public void SetGrabbed(bool isGrabbed, float inputMoveDelta)
    {
        mCurrentOrbitSpeed = (isGrabbed) ? 0.0f : DefaultOrbitSpeed;
        mLastGrabInputDelta = inputMoveDelta;

        if (isGrabbed)
        {
            // Check where we're grabbing in world space (on the xz-plane only)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out mRayCastHit, 1000.0f, GameManager.Instance.XZPlaneCollisionMask))
            {
                // Calculate the nearest point on the oribtable's circular orbit
                var universeToGrabPos = (mRayCastHit.point - mCenterOfTheUniverse).normalized;
                var newPosition = mCenterOfTheUniverse + universeToGrabPos * OrbitDistance;

                CalculateOrbitDirection(transform.position, newPosition);

                // Go straight to that point! Update ensures we are always looking the right way.
                transform.position = newPosition;

                // Keep track of input delta that we might want to apply to our velocity
                // if the user releases the mouse button
                mExtraOrbitSpeed += (inputMoveDelta * ExtraOrbitSpeedInputMultiplier);
                mExtraOrbitSpeed = Mathf.Clamp(mExtraOrbitSpeed, -MaxOrbitSpeed, MaxOrbitSpeed);
                mLastDragInputTime = Time.timeSinceLevelLoad;
            }
        }
        else if (mGrabbed && !isGrabbed && mExtraOrbitSpeed != 0.0f)
        {
            if ((Time.timeSinceLevelLoad - mLastDragInputTime) < TimeBeforeResetExtraOrbitSpeed)
            {
                // Work out in which direction to apply our extra orbital movement
                // Also make sure we have at least a minimum amount (feels nicer)
                mExtraOrbitSpeed = Mathf.Max(MinExtraOrbitSpeed, mExtraOrbitSpeed);
                mExtraOrbitSpeed *= mOrbitDirectionMultipler;

                // Jump straight to our extra speed and then tween back down to default speed over time
                mCurrentOrbitSpeed = mExtraOrbitSpeed;
                var tweenTime = ExtraOrbitSpeedFalloffTimeCurve.Evaluate(Mathf.Abs(mExtraOrbitSpeed));
                LeanTween.value(gameObject, mCurrentOrbitSpeed, DefaultOrbitSpeed, tweenTime).setEaseInOutElastic().setOnUpdate((float val) =>
                {
                    mCurrentOrbitSpeed = val;
                }).setOnComplete(() =>
                {
                    mExtraOrbitSpeed = 0.0f;
                });
            }
            else
            {
                mExtraOrbitSpeed = 0.0f;
            }
        }

        mGrabbed = isGrabbed;
    }

    // -------------------------------------------------------------------------------

    public void SetAngle(float angleInDegrees, float duration, bool pauseOnComplete, System.Action onCompleteCallback = null)
    {
        if (mTweenDescr != null)
        {
            LeanTween.cancel(mTweenDescr.id);
        }

        var targetAngle = angleInDegrees * Mathf.Deg2Rad;
        var currentAngleRadians = (mDirectionAngle -90f) * Mathf.Deg2Rad;

        // Force clockwise rotation
        if (targetAngle < currentAngleRadians)
        {
            targetAngle += (Mathf.PI * 2.0f);
        }

        mTweenDescr = LeanTween.value(gameObject, (float angle) =>
        {
            Vector3 toCenter = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
            transform.position = mCenterOfTheUniverse - (toCenter * OrbitDistance);

            foreach (var linkedOrbitable in LinkedOribtables)
            {
                linkedOrbitable.FollowLinkedOrbitable(this);
            }
        },
        currentAngleRadians, targetAngle, duration).setOnComplete(() =>
        {
            if (pauseOnComplete)
            {
                IsPaused = true;
            }

            mTweenDescr = null;
            if (onCompleteCallback != null)
            {
                onCompleteCallback.Invoke();
            }
        });
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        SetGrabbed(true, eventData.delta.sqrMagnitude);
    }

    // -------------------------------------------------------------------------------

    public void OnEndDrag(PointerEventData eventData)
    {
        SetGrabbed(false, 0.0f);
    }

    // -------------------------------------------------------------------------------

    private void CalculateDirectionAngle()
    {
        Vector3 toCenter = (mCenterOfTheUniverse - transform.position);
        mDirectionAngle = (Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg) + 90.0f;

        // Make sure the angle is always positive, where 0 is directly above the center of
        // the universe and clockwise rotations increase our angle
        if (mDirectionAngle < 0.0f)
        {
            mDirectionAngle = (360.0f + mDirectionAngle);
        }
    }

    // -------------------------------------------------------------------------------

    private void CalculateOrbitDirection(Vector3 prevPosition, Vector3 newPosition)
    {
        if (prevPosition != newPosition)
        {
            var prevAngle = Vector3.Angle(Vector3.up, (prevPosition - mCenterOfTheUniverse));
            var newAngle = Vector3.Angle(Vector3.up, (newPosition - mCenterOfTheUniverse));

            var prevOnLeftSide = prevPosition.x < 0.0f;
            var newOnLeftSide = newPosition.x < 0.0f;

            // If we're still on the same side, check the change in angle to determine orbit direction
            // Ignore side flips and wait for next check to correct any changes
            if (prevOnLeftSide == newOnLeftSide)
            {
                if (prevOnLeftSide)
                {
                    mOrbitDirectionMultipler = (newAngle > prevAngle) ? 1.0f : -1.0f;
                }
                else
                {
                    mOrbitDirectionMultipler = (newAngle < prevAngle) ? 1.0f : -1.0f;
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
}
