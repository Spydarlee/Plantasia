using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController      Instance = null;

    [Header("Cinemachine")]
    public CinemachineBrain                 CinemachineBrain = null;
    public CinemachineVirtualCamera         MainCamera = null;
    public List<CinemachineVirtualCamera>   AllVirtualCameras = new List<CinemachineVirtualCamera>();

    [Header("Zoom Settings")]
    public float            ZoomInputMultiplier = 10.0f;
    public float            ZoomTouchInputMultiplier = 0.1f;
    public float            ZoomSmoothTime = 0.25f;
    public float            MinDistance = -5.0f;
    public float            MaxDistance = -30.0f;

    [Header("LookAt Settings")]
    public Vector3          LookAtTarget = Vector3.zero;
    public float            LookAtSmoothTime = 0.25f;

    // -------------------------------------------------------------------------------

    private Vector3         mPreOverridePosition = Vector3.zero;
    private Quaternion      mPreOverrideRotation = Quaternion.identity;
    private Vector3         mCurrentLookAtTarget = Vector3.zero;
    private float           mNormalisedZoomDistance = 0.0f;
    private float           mTargetDistance = 0.0f;
    private float           mHeightOffset = 0.0f;
    private bool            mIsBeingOverriden = false;
    private Vector3         mZoomVelocity = Vector3.zero;
    private Vector3         mLookAtVelocity = Vector3.zero;
    private UIManager       mUIManager = null;

    // -------------------------------------------------------------------------------

    public Vector3          PreOverridePosition { get { return mPreOverridePosition; } }
    public Quaternion       PreOverrideRotation { get { return mPreOverrideRotation; } }
    public bool             DisableZoomInput { get; set; }
    public float            TargetDistance { get { return mTargetDistance; } set { mTargetDistance = value; } }

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        if (CinemachineBrain == null)
        {
            CinemachineBrain = GetComponent<CinemachineBrain>();
        }

        if (MainCamera == null)
        {
            MainCamera = CinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        }

        mTargetDistance = Mathf.Abs(MainCamera.transform.position.z);
        mHeightOffset = Mathf.Abs(MainCamera.transform.position.y);
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        mUIManager = UIManager.Instance;
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        if (!mIsBeingOverriden)
        {
            // When we're fully zoomed OUT we look at the origin
            // When we're fully zoomed IN we look at our LookAtTarget
            var desiredLookAtTarget = Vector3.Lerp(LookAtTarget, Vector3.zero, mNormalisedZoomDistance);

            // If our LookAtTarget changes we smoothly tween to the new value at a given speed
            if (desiredLookAtTarget != mCurrentLookAtTarget)
            {
                mCurrentLookAtTarget = Vector3.SmoothDamp(mCurrentLookAtTarget, desiredLookAtTarget, ref mLookAtVelocity, LookAtSmoothTime);
                MainCamera.transform.LookAt(mCurrentLookAtTarget);
            }                        

            if (!DisableZoomInput && (mUIManager == null || !mUIManager.IsCursorOverUI))
            {
                UpdateTargetDistance();

                // Move back along the Z-axis by the new distance value
                var targetPosition = mCurrentLookAtTarget;
                targetPosition.y += mHeightOffset;
                targetPosition.z = -mTargetDistance;

                MainCamera.transform.position = Vector3.SmoothDamp(MainCamera.transform.position, targetPosition, ref mZoomVelocity, ZoomSmoothTime);
                mNormalisedZoomDistance = Mathf.InverseLerp(MinDistance, MaxDistance, Mathf.Abs(MainCamera.transform.position.z));
            }           
        }
    }

    // -------------------------------------------------------------------------------

    public void SetOverrideMode(bool overrideMode, bool restorePrevTransform = true)
    {
        if (overrideMode)
        {
            mPreOverridePosition = transform.position;
            mPreOverrideRotation = transform.rotation;
        }
        else if(restorePrevTransform)
        {
            transform.position = mPreOverridePosition;
            transform.rotation = mPreOverrideRotation;
        }

        mIsBeingOverriden = overrideMode;
    }

    // -------------------------------------------------------------------------------

    public void DisableAllVirtualCameras(CinemachineVirtualCamera cameraToIgnore)
    {
        foreach (var camera in AllVirtualCameras)
        {
            if (camera != cameraToIgnore)
            {
                camera.gameObject.SetActive(false);
            }            
        }
    }

    // -------------------------------------------------------------------------------

    private bool UpdateTargetDistance()
    {
        float newTargetDistance = mTargetDistance;

        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            newTargetDistance += deltaMagnitudeDiff * ZoomTouchInputMultiplier;
            newTargetDistance = Mathf.Clamp(newTargetDistance, MinDistance, MaxDistance);
        }
        else
        {
            newTargetDistance -= Input.GetAxis("Mouse ScrollWheel") * ZoomInputMultiplier;
            newTargetDistance = Mathf.Clamp(newTargetDistance, MinDistance, MaxDistance);
        }

        if (newTargetDistance != mTargetDistance)
        {
            mTargetDistance = newTargetDistance;
            return true;
        }

        return false;
    }

    // -------------------------------------------------------------------------------

    private void CancelTween(ref int tweenId)
    {
        LeanTween.cancel(tweenId);
        tweenId = -1;
    }

    // -------------------------------------------------------------------------------
}
