using Cinemachine;
using ActionGraph;
using UnityEngine;

public class SwapHomePlanetoidState : Action
{
    // -------------------------------------------------------------------------------

    public CinemachineVirtualCamera ProxyTransitionInCamera = null;
    public CinemachineVirtualCamera ProxyTransitionOutCamera = null;

    // -------------------------------------------------------------------------------

    private GameManager mGameManager = null;
    private Universe mUniverse = null;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mUniverse = mGameManager.Universe;

        // When we reach this state the camera is zoomed into the proxy and
        // we want to swap in the proper planetoid and focus on that for the outro

        // Swap the old planetoid for the new one
        var newPlanetoid = mGameManager.SelectedPlanetoidProxy.Planetoid;
        mUniverse.ChooseNewPlanetoid(newPlanetoid);
        newPlanetoid.gameObject.SetActive(true);

        if (newPlanetoid.Flipable)
        {
            newPlanetoid.Flipable.SetUpsideDown(false);
        }

        // Make sure the new planetoid takes ownership of all the proxies
        newPlanetoid.SetupProxies(ref mUniverse.PlanetoidProxies);

        // Snap the transition out camera to look at the new planetoid
        ProxyTransitionOutCamera.Follow = newPlanetoid.transform;
        ProxyTransitionOutCamera.LookAt = newPlanetoid.transform;
        ProxyTransitionOutCamera.gameObject.SetActive(true);
        ProxyTransitionInCamera.gameObject.SetActive(false);

        // Snap the seed ship to this new planetoid
        mGameManager.SeedShip.PlanetoidOwner = mGameManager.CurrentPlanetoid;
        mGameManager.SeedShip.SetAttachedToPlanetoid(true);
        mGameManager.SeedShip.transform.localPosition = Vector3.zero;
        mGameManager.SeedShip.transform.localRotation = Quaternion.identity;

        mGameManager.SelectedPlanetoidProxy = null;
        mGameManager.ChangeGameplayState(GameplayStates.Planetoid);
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}