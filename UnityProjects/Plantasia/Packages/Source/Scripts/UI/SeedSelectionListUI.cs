using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSelectionListUI : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public SlidingPanelUI       SlidingPanelOwner;
    public GameObject           SeedUIPrefab;

    // -------------------------------------------------------------------------------

    private List<SeedUI>        mSeedUIElements = new List<SeedUI>();

    // -------------------------------------------------------------------------------

    private void Start()
    {
        if (SlidingPanelOwner != null)
        {
            SlidingPanelOwner.OnSlideOn += OnSlideOn;
        }
    }

    // -------------------------------------------------------------------------------

    void OnSlideOn()
    {
        foreach (var seedUIElement in mSeedUIElements)
        {
            GameObjectPooler.Instance.ReturnToPool(seedUIElement.gameObject);
        }

        mSeedUIElements.Clear();
        SetupSeedUIElements();
    }

    // -------------------------------------------------------------------------------

    private void SetupSeedUIElements()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        var seedShip = GameManager.Instance.SeedShip;

        // Create a SeedUI element for each type of seed we have in the seedship
        foreach (PlantTypes plantType in Enum.GetValues(typeof(PlantTypes)))
        {
            int seedCount = seedShip.GetSeedCount(plantType);
            if (seedCount > 0)
            {
                // Create the new button (or repurpose an existing one)
                var seedUI = GameObjectPooler.Instance.GetPooledInstance(SeedUIPrefab);
                var seedUIComp = seedUI.GetComponent<SeedUI>();

                // Initialise the new UI component with the appropraite parent, sprite, etc.
                seedUI.transform.SetParent(transform);
                seedUI.transform.localScale = Vector3.one;
                seedUIComp.Initialise(plantType, seedCount);
                mSeedUIElements.Add(seedUIComp);
            }
        }
    }

    // -------------------------------------------------------------------------------
}
