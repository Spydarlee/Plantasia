using UnityEngine;
using UnityEngine.EventSystems;

public class SeedShipInterior : MonoBehaviour, IDragHandler
{
    // -------------------------------------------------------------------------------

    public SeedManipulator  SeedManipulator = null;
    public Transform        SeedSpawnPos = null;
    public Transform        SeedEntryExitPos = null;
    public Camera           InteriorCamera = null;
    public Transform        InteriorCameraOrientation = null;
    public Camera           RenderTextureCamera = null;

    // -------------------------------------------------------------------------------

    public SeedShip         Owner { get; set; }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        DelayedOptimisations();
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == -1 && SeedManipulator != null)
        {
            SeedManipulator.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        }
    }

    // -------------------------------------------------------------------------------

    public void RequestExitInterior()
    {
        Owner.ExitInterior();
    }

    // -------------------------------------------------------------------------------

    private void OnGameStateChanged(GameplayStates prevState, GameplayStates newState)
    {
        if (newState == GameplayStates.InsideSeedShip)
        {
            Owner.SetSeedPhysicsEnabled(true);
        }
        else if (prevState == GameplayStates.InsideSeedShip)
        {
            RenderTextureCamera.enabled = true;
            DelayedOptimisations();
        }
    }

    // -------------------------------------------------------------------------------

    private void DelayedOptimisations()
    {
        LeanTween.delayedCall(2.0f, () =>
        {
            Owner.SetSeedPhysicsEnabled(false);
            RenderTextureCamera.enabled = false;
        });
    }

    // -------------------------------------------------------------------------------
}
