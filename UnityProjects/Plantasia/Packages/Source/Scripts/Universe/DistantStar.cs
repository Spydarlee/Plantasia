using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DistantStar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public Renderer                     ModelRenderer = null;
    public List<Renderer>               FeatureRenderers = new List<Renderer>();
    public string                       MaterialColorPropertyName = "_Color";
    public Planetoid.PlanetoidTypes     PlanetoidType = Planetoid.PlanetoidTypes.Default;
    public bool                         RotateToFaceOrigin = true;

    public Universe                     Universe { get; set; }
    public Planetoid                    Planetoid { get; set; }

    // -------------------------------------------------------------------------------

    private Collider    mCollider = null;
    private Color       mTintColor = Color.white;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        if (ModelRenderer == null)
        {
            ModelRenderer = GetComponentInChildren<Renderer>();
        }

        mCollider = GetComponent<Collider>();
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        if (RotateToFaceOrigin)
        {
            transform.LookAt(Vector3.zero);
        }        
    }

    // -------------------------------------------------------------------------------

    public void SetTintColor(Color tintcolor)
    {
        if (ModelRenderer)
        {
            mTintColor = tintcolor;
            ModelRenderer.material.SetColor(MaterialColorPropertyName, tintcolor);
        }

        foreach (var renderer in FeatureRenderers)
        {
            renderer.material.SetColor(MaterialColorPropertyName, tintcolor);
        }
    }

    // -------------------------------------------------------------------------------

    public void Show(bool instant = false)
    {
        mCollider.enabled = true;

        if (instant)
        {
            mTintColor.a = 1.0f;
            SetTintColor(mTintColor);
        }
        else
        {
            LeanTween.value(gameObject, 0.0f, 1.0f, 1.0f).setOnUpdate((float alpha) =>
            {
                mTintColor.a = alpha;
                SetTintColor(mTintColor);
            });
        }
    }

    // -------------------------------------------------------------------------------

    public void Hide(bool instant = false)
    {
        mCollider.enabled = false;

        if (instant)
        {
            mTintColor.a = 0.0f;
            SetTintColor(mTintColor);
        }
        else
        {
            LeanTween.value(gameObject, mTintColor.a, 0.0f, 1.0f).setOnUpdate((float alpha) =>
            {
                mTintColor.a = alpha;
                SetTintColor(mTintColor);
            });
        }
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Doesn't do anything, just here for the OnPointerUp event!
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!eventData.dragging)
        {
            if (Universe.CurrentPlanetoid == Planetoid)
            {
                GameManager.Instance.SeedShip.SetAttachedToPlanetoid(false);
                GameManager.Instance.ChangeGameplayState(GameplayStates.Planetoid);
            }
            else
            {
                Universe.ChooseNewPlanetoid(Planetoid);
            }
        }
    }

    // -------------------------------------------------------------------------------
}
