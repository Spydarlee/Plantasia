using ActionGraph;
using UnityEngine;

public class PlanetoidStateAction : Action
{
    // -------------------------------------------------------------------------------

    private GameManager mGameManager = null;
    private Rotatable   mPlanetoidRotatable = null;
    private Flipable    mPlanetoidFlipable = null;
    private UIManager   mUIManager = null;
    private Ray         mRay = new Ray();
    private RaycastHit  mRaycastHit;
    private LayerMask   mPlanetoidLayerMask = 0;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mGameManager.SeedShip.PlanetoidOwner = mGameManager.CurrentPlanetoid;
        mGameManager.SeedShip.SetAttachedToPlanetoid(true);

        mPlanetoidFlipable = mGameManager.CurrentPlanetoid.Flipable;
        mPlanetoidRotatable = mGameManager.CurrentPlanetoid.Rotatable;
        mPlanetoidRotatable.DisableInput = false;

        mUIManager = UIManager.Instance;
        mUIManager.SeedSelectionUI.Show();

        mRay.origin = Vector3.up * 100.0f;
        mRay.direction = Vector3.down;
        mPlanetoidLayerMask = PlanetoidObjectSpawnHelper.Instance.PlanetoidLayerMask;
    }

    // -------------------------------------------------------------------------------

    protected override void OnUpdate()
    {
        if (mPlanetoidFlipable == null || !mPlanetoidFlipable.IsFlipping)
        {
            if (Physics.Raycast(mRay, out mRaycastHit, 1000.0f, mPlanetoidLayerMask, QueryTriggerInteraction.Ignore))
            {
                CameraController.Instance.LookAtTarget = mRaycastHit.point;
            }
        }    
    }

    // -------------------------------------------------------------------------------

    protected override void OnFinish()
    {
        mPlanetoidRotatable.DisableInput = true;
        mUIManager.SeedSelectionUI.Hide();
        mUIManager.CurrentSeedUI.Hide();
        mGameManager.SeedShip.CurrentSeed = null;
        CameraController.Instance.LookAtTarget = Vector3.zero;
    }

    // -------------------------------------------------------------------------------
}
