using UnityEngine;
using UnityEngine.UI;

public class PlantGrowthStatusUI : UIElement
{
    // -------------------------------------------------------------------------------

    public Image    BackgroundImage = null;
    public Image    ForegroundImage = null;
    public Image    FrameImage = null;
    public Color    FrameColorRequirementMet = Color.white;
    public Color    FrameColorRequirementNotMet = Color.gray;
    public float    ScaleTweenDuration = 1.0f;

    // -------------------------------------------------------------------------------

    private Plant                   mPlant;
    private PlantGrowthRequirement  mGrowthRequirement;
    private Vector3                 mScale = Vector3.one;

    // -------------------------------------------------------------------------------

    public void Update()
    {
        if (IsVisible)
        {
            var requirementStatus = mGrowthRequirement.GetStatusAsPercentage(mPlant);
            ForegroundImage.fillAmount = requirementStatus;
            FrameImage.color = Color.Lerp(FrameColorRequirementNotMet, FrameColorRequirementMet, requirementStatus);
        }
    }

    // -------------------------------------------------------------------------------

    public void Show(Plant plant, PlantGrowthRequirement growthRequirement)
    {
        mPlant = plant;
        mGrowthRequirement = growthRequirement;

        BackgroundImage.sprite = mGrowthRequirement.UISprite;
        ForegroundImage.sprite = mGrowthRequirement.UISprite;

        Show(false);
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        TweenScale(0.0f, 1.0f);
        IsVisible = true;
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (IsVisible)
        {
            if (instant)
            {
                SetScale(Vector3.zero);
            }
            else
            {
                TweenScale(1.0f, 0.0f);
            }

            IsVisible = false;
            mPlant = null;
            mGrowthRequirement = null;
        }
    }

    // -------------------------------------------------------------------------------

    private void TweenScale(float from, float to)
    {
        LeanTween.value(from, to, ScaleTweenDuration).setOnUpdate((float value) =>
        {
            mScale.x = mScale.y = mScale.z = value;
            SetScale(mScale);
        });
    }

    // -------------------------------------------------------------------------------

    private void SetScale(Vector3 scale)
    {
        mScale = scale;
        FrameImage.transform.localScale = mScale;
        BackgroundImage.transform.localScale = mScale;
        ForegroundImage.transform.localScale = mScale;
    }

    // -------------------------------------------------------------------------------
}
