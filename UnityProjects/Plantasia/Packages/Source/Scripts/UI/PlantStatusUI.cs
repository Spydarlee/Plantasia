using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantStatusUI : UIElement
{
    // -------------------------------------------------------------------------------

    public static PlantStatusUI         Instance = null;
    public List<PlantGrowthStatusUI>    GrowthStatusUIElements;
    public UIElement                    RemovePlantButton = null;
    public UIElement                    FullyGrownUIElement = null;
    public float                        DelayAfterInputBeforeHide = 0.1f;

    // -------------------------------------------------------------------------------

    private Plant   mPlant = null;
    private float   mHideCountdown = 0.0f;
    private Vector3 mMouseDownPosition = Vector3.zero;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------------------

    private void Update()
    {
        if (mIsVisible)
        {
            if (mHideCountdown > 0.0f)
            {
                mHideCountdown -= Time.deltaTime;
                if (mHideCountdown <= 0.0f)
                {
                    Hide();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mMouseDownPosition = Input.mousePosition;
                }

                // We want to hide ourselves if we detect *any* input, unless that input
                // is trying to show us, so wait for just a moment to see..
                var uiManager = UIManager.Instance;
                if (Input.GetMouseButtonUp(0) && Input.mousePosition == mMouseDownPosition && 
                    !uiManager.IsCursorOverUI && !uiManager.AboutToShowPlantStatusUI)
                {
                    mHideCountdown = DelayAfterInputBeforeHide;
                }
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void Show(Plant plant)
    {
        mHideCountdown = 0.0f;
        bool plantHasChanged = (mPlant == null || mPlant != plant);
        mPlant = plant;

        if (!IsVisible)
        {
            Show();
        }
        else if (plantHasChanged)
        {
            // If we're already visible, then refresh
            OnShow();
        }
    }

    // -------------------------------------------------------------------------------

    public void RemovePlant()
    {
        if (mPlant != null)
        {
            mPlant.HarvestSeed(true);
            Hide();
        }
    }

    // -------------------------------------------------------------------------------

    public void NotifyPlantChanged(Plant plant)
    {
        if (mPlant == plant && IsVisible)
        {
            OnShow();
        }
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        if (mPlant.IsFullyGrown)
        {
            foreach (var growthStatusUI in GrowthStatusUIElements)
            {
                growthStatusUI.Hide();
            }

            RemovePlantButton.Hide();

            FullyGrownUIElement.Show();
        }
        else
        {
            if (FullyGrownUIElement.IsVisible)
            {
                FullyGrownUIElement.Hide();
            }

            var numRequirements = mPlant.PlantData.GrowthRequirements.Count;

            // If we already had some UI up, make sure we turn off any leftovers
            for (int i = numRequirements; i < GrowthStatusUIElements.Count; i++)
            {
                var leftoverElement = GrowthStatusUIElements[i];
                if (leftoverElement.IsVisible)
                {
                    leftoverElement.Hide();
                }
            }

            for (int i = 0; i < numRequirements; i++)
            {
                GrowthStatusUIElements[i].Show(mPlant, mPlant.PlantData.GrowthRequirements[i]);
            }

            if (Universe.Instance.CurrentPlanetoid.IsHomePlanetoid)
            {
                RemovePlantButton.Show();
            }            
        }

        IsVisible = true;
        mHideCountdown = 0.0f;
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        foreach (var growthStatusUI in GrowthStatusUIElements)
        {
            growthStatusUI.Hide(instant);
        }

        FullyGrownUIElement.Hide(instant);
        RemovePlantButton.Hide(instant);

        mIsVisible = false;
        mHideCountdown = 0.0f;
        mPlant = null;
    }

    // -------------------------------------------------------------------------------
}
