
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class PlanetoidProxy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public Planetoid                    Planetoid;
    public CinemachineVirtualCamera     TransitionInCamera = null;
    public Collider                     Collider = null;
    private GameManager                 mGameManager = null;

    // -------------------------------------------------------------------------------

    public delegate void ProxyClickedAction();
    public event ProxyClickedAction OnProxyClicked = null;

    // -------------------------------------------------------------------------------

    public void Awake()
    {
        if (Collider == null)
        {
            Collider = GetComponent<Collider>();
        }
    }

    // -------------------------------------------------------------------------------

    public void Start()
    {
        mGameManager = GameManager.Instance;
    }

    // -------------------------------------------------------------------------------

    public void SetActive(bool active)
    {
        Collider.enabled = active;
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Does nothing, just here so we can get OnPointerUp
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mGameManager.CurrentGameplayState == GameplayStates.Planetoid)
        {
            if (!eventData.dragging && mGameManager.Universe.IsOnHomePlanetoid)
            {
                // Make sure the transition camera knows we're the one to look at!
                TransitionInCamera.Follow = transform;
                TransitionInCamera.LookAt = transform;

                // Fire of that state transition, yo
                mGameManager.SelectedPlanetoidProxy = this;
                mGameManager.ChangeGameplayState(GameplayStates.SwappingHomePlanetoids);
            }
        }
        else if(mGameManager.CurrentGameplayState == GameplayStates.FoundBuriedObject)
        {
            if (!eventData.dragging && OnProxyClicked != null)
            {
                OnProxyClicked.Invoke();
            }
        }
    }

    // -------------------------------------------------------------------------------
}
