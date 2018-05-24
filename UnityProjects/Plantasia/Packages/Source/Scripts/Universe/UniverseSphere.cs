using UnityEngine;
using UnityEngine.EventSystems;

public class UniverseSphere : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public Universe         Universe = null;
    public float            Radius = 50.0f;
    public UniverseMarker   HomePlanetoidMarker = null;
    public UniverseMarker   CurrentPlanetoidMarker = null;
    public bool             EnableCurrentPlanetoidMarker = false;

    // -------------------------------------------------------------------------------

    public bool IsVisible { get { return gameObject.activeInHierarchy && mRenderer.material.color.a >= 1.0f; } }

    // -------------------------------------------------------------------------------

    private Vector3     mPrevMousePosition = Vector3.zero;
    private Rotatable   mRotatable = null;
    private Renderer    mRenderer = null;

    // -------------------------------------------------------------------------------

    public void Awake()
    {
        mRotatable = GetComponent<Rotatable>();
        mRenderer = GetComponent<Renderer>();
    }

    // -------------------------------------------------------------------------------

    public void OnEnable()
    {
        mPrevMousePosition = Input.mousePosition;
    }

    // -------------------------------------------------------------------------------

    public void Show()
    {
        LeanTween.value(0.0f, 1.0f, 1.0f).setOnUpdate((float value) =>
        {
            var color = mRenderer.material.color;
            color.r = color.g = color.b = value;
            mRenderer.material.color = color;
        });

        if (Universe.CurrentPlanetoid != Universe.CurrentHomePlanetoid)
        {
            HomePlanetoidMarker.gameObject.SetActive(true);
            HomePlanetoidMarker.Update();
            HomePlanetoidMarker.Target = Universe.CurrentHomePlanetoid.DistantStar.gameObject;
        }

        if (EnableCurrentPlanetoidMarker && Universe.CurrentHomePlanetoid != Universe.CurrentPlanetoid)
        {
            CurrentPlanetoidMarker.gameObject.SetActive(true);
            CurrentPlanetoidMarker.Update();
            CurrentPlanetoidMarker.Target = Universe.CurrentPlanetoid.DistantStar.gameObject;
        }
    }

    // -------------------------------------------------------------------------------

    public void Hide()
    {
        HomePlanetoidMarker.Hide();

        if(EnableCurrentPlanetoidMarker)
            CurrentPlanetoidMarker.Hide();

        LeanTween.value(1.0f, 0.0f, 1.0f).setOnUpdate((float value) =>
        {
            var color = mRenderer.material.color;
            color.r = color.g = color.b = value;
            mRenderer.material.color = color;

        }).setOnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        // The Universe sphere doesn't have collision becaue we're *inside* the sphere
        // which messes up Raycasts and input events, so calculate input drag manually..
        if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
        {
            var mouseDelta = Input.mousePosition - mPrevMousePosition;
            if (mouseDelta.sqrMagnitude > 0.0f)
            {
                mRotatable.Rotate(mouseDelta);
            }
        }

        mPrevMousePosition = Input.mousePosition;

        // Make sure the home planetoid's star always has the same flat rotation
        Universe.CurrentHomePlanetoid.DistantStar.transform.up = Vector3.up;
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        mPrevMousePosition = Input.mousePosition;
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    // -------------------------------------------------------------------------------
}
