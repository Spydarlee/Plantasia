using Cinemachine;
using ActionGraph;
using UnityEngine;

public class RidingSunbeamsState : Action
{

    // -------------------------------------------------------------------------------

    public CinemachineVirtualCamera StateCamera;
    public StarField                StarField;
    public float                    MaxTransformBlendSpeed = 10.0f;
    public float                    TimeToReachMaxTransformBlendSpeed = 1.0f;
    public float                    StateDuration = 5.0f;
    public Vector3                  ShipRotation = new Vector3(45f, 0f, 0f);

    // -------------------------------------------------------------------------------

    private SeedShip    mSeedShip = null;
    private Ray         mRay;
    private RaycastHit  mRayCastHit;
    private GameManager mGameManager = null;
    private Vector3     mCenterOfTheUniverse = Vector3.zero;
    private float       mCurrentTransformBlendSpeed = 0.0f;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mSeedShip = mGameManager.SeedShip;

        mGameManager.CurrentPlanetoid.gameObject.SetActive(false);
        mSeedShip.SetAttachedToPlanetoid(false);
        StarField.SetEnabled(true);

        // Stop tracking the ship while we're in this state
        StateCamera.gameObject.SetActive(true);
        StateCamera.Follow = null;
        StateCamera.LookAt = null;

        var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (CastAgainstXZPlane(screenCenter))
        {
            mCenterOfTheUniverse = mRayCastHit.point;
        }

        mCurrentTransformBlendSpeed = 0.0f;
    }

    // -------------------------------------------------------------------------------

    protected override void OnUpdate()
    {
        Vector3 shipUp = (mCenterOfTheUniverse - mSeedShip.transform.position);
        Vector3 shipRotation = ShipRotation;

        // Ease into our transform blends so there's a smooth transition to start with
        if (mElapsedTime < TimeToReachMaxTransformBlendSpeed)
        {
            var t = (mElapsedTime / TimeToReachMaxTransformBlendSpeed);
            mCurrentTransformBlendSpeed = Mathf.Lerp(mCurrentTransformBlendSpeed, MaxTransformBlendSpeed, t);
            shipRotation = Vector3.Lerp(Vector3.zero, ShipRotation, t);
            shipUp = Vector3.Lerp(Vector3.up, shipUp.normalized, t);
        }

        if (CastAgainstXZPlane(Input.mousePosition))
        {
            // Move the seedship to the cursor position (in world space)
            mSeedShip.transform.position = Vector3.Lerp(mSeedShip.transform.position, mRayCastHit.point, mCurrentTransformBlendSpeed * Time.deltaTime);

            // Angle the ship so it always points towards the center of the screen
            mSeedShip.transform.up = shipUp;
            mSeedShip.transform.Rotate(shipRotation);
        }

        if (mElapsedTime >= StateDuration)
        {
            mGameManager.ChangeGameplayState(GameplayStates.Planetoid);
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------

    protected override void OnFinish()
    {
        // Return to tracking the ship for the next time round
        StateCamera.Follow = mSeedShip.transform;
        StateCamera.LookAt = mSeedShip.transform;

        StarField.SetEnabled(false);
    }

    // -------------------------------------------------------------------------------

    private bool CastAgainstXZPlane(Vector2 screenPos)
    {
        mRay = Camera.main.ScreenPointToRay(screenPos);
        return Physics.Raycast(mRay, out mRayCastHit, 1000.0f, mGameManager.XZPlaneCollisionMask);
    }

    // -------------------------------------------------------------------------------
}