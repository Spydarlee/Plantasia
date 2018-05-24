using UnityEngine;
using UnityEngine.UI;

public class SeedUI : UIElement
{
    // -------------------------------------------------------------------------------

    public Image        SeedImage = null;
    public Text         SeedCountText = null;
    public string       OnSelectedSfx = AudioManager.InvalidSfxName;
    public string       OnDeselectedSfx = AudioManager.InvalidSfxName;

    public bool         ScaleUpOnUpdated = false;
    public Vector3      OnUpdateTweenScale = new Vector3(0.5f, 0.5f, 0.5f);

    // -------------------------------------------------------------------------------

    private PlantTypes  mPlantType = PlantTypes.Snowdrop;
    private bool        mIsTweening = false;

    // -------------------------------------------------------------------------------

    public void Initialise(PlantTypes plantType, int count)
    {
        mPlantType = plantType;
        SeedImage.sprite = PlantManager.Instance.GetPlantData(plantType).SeedSprite;
        SeedCountText.text = count.ToString();

        // If we've just spawned a plant then let's jiggle
        // around a bit to make sure the player knows they've used a seed
        if (ScaleUpOnUpdated && !mIsTweening && mIsVisible && count > 0)
        {
            mIsTweening = true;

            LeanTween.scale(gameObject, OnUpdateTweenScale, 0.25f).setOnComplete(() =>
            {
                LeanTween.scale(gameObject, Vector3.one, 0.4f).setEaseOutBounce().setOnComplete(() =>
                {
                    mIsTweening = false;
                });
            });
        }
    }

    // -------------------------------------------------------------------------------

    public void OnSelected()
    {
        if (OnSelectedSfx != AudioManager.InvalidSfxName)
        {
            AudioManager.Instance.PlaySfx(null, OnSelectedSfx);
        }

        GameManager.Instance.SeedShip.SelectCurrentSeedType(mPlantType);
        UIManager.Instance.SeedSelectionUI.SlideOff();
    }

    // -------------------------------------------------------------------------------

    public void OnDeselected()
    {
        if (OnDeselectedSfx != AudioManager.InvalidSfxName)
        {
            AudioManager.Instance.PlaySfx(null, OnDeselectedSfx);
        }

        GameManager.Instance.SeedShip.CurrentSeed = null;
    }

    // -------------------------------------------------------------------------------
}
