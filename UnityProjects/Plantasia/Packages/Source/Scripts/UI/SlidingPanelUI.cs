using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlidingPanelUI : UIElement, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public Vector2              OffScreenPosition = Vector2.zero;
    public Vector2              OnScreenPosition = Vector2.zero;

    [Header("Hide Settings")]
    public bool                 DisableWhenOffScreen = false;
    public bool                 FadeOutWhenOffScreen = false;
    public Image                BackgroundImage = null;
    public float                OffScreenBGImageAlpha = 0.5f;
    public float                TimeBeforeAutoSlideOff = 1.5f;
    public float                TimeBeforeAutoSlideOffMobile = 3.0f;

    [Header("Interaction Settings")]
    public SlidingPanelUIProxy  SlidingPanelProxy = null;
    public SwipeCheckData       SwipeToCloseData = null;
    public SwipeCheckData       SwipeToOpenData = null;
    public bool                 TriggerOnClick = true;
    public string               SlideOnSfx = AudioManager.InvalidSfxName;
    public string               SlideOffSfx = AudioManager.InvalidSfxName;

    // -------------------------------------------------------------------------------

    public delegate void SlideOnAction();
    public event SlideOnAction OnSlideOn = null;

    public bool IsOnScreen { get { return mIsCurrentlyOnScreen; } }

    // -------------------------------------------------------------------------------

    private RectTransform   mRectTransform = null;
    private bool            mIsTransitioning = false;
    private float           mTimeSinceUnderPointer = 0.0f;
    private bool            mIsCurrentlyOnScreen = false;
    private int             mAlphaTweenId = -1;
    private float           mTimeBeforeAutoSlideOff = 3.0f;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
        mTimeBeforeAutoSlideOff = (Application.isMobilePlatform) ? TimeBeforeAutoSlideOffMobile : TimeBeforeAutoSlideOff;
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        if (!mIsUnderPointer && !mIsTransitioning && mIsCurrentlyOnScreen)
        {
            mTimeSinceUnderPointer += Time.deltaTime;
            if (mTimeSinceUnderPointer >= mTimeBeforeAutoSlideOff)
            {
                SlideOff();
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void SlideOn()
    {
        if (mIsTransitioning || mIsCurrentlyOnScreen)
            return;

        mIsTransitioning = true;

        if (OnSlideOn != null)
        {
            OnSlideOn.Invoke();
        }

        if (SlideOnSfx != AudioManager.InvalidSfxName)
        {
            AudioManager.Instance.PlaySfx(null, SlideOnSfx);
        }

        if (DisableWhenOffScreen)
        {
            gameObject.SetActive(true);
        }
        else if (FadeOutWhenOffScreen && BackgroundImage != null)
        {
            TweenBGImageAlpha(BackgroundImage.color.a, 1.0f);
        }

        LeanTween.move(mRectTransform, OnScreenPosition, ToggleDuration).setOnComplete(() =>
        {
            mIsTransitioning = false;
            mIsCurrentlyOnScreen = true;
        });
    }

    // -------------------------------------------------------------------------------

    public void SlideOff()
    {
        if (mIsTransitioning || !mIsCurrentlyOnScreen || !IsEnabled)
            return;

        mIsTransitioning = true;
        mTimeSinceUnderPointer = 0.0f;

        if (SlideOffSfx != AudioManager.InvalidSfxName)
        {
            AudioManager.Instance.PlaySfx(null, SlideOffSfx);
        }

        LeanTween.move(mRectTransform, OffScreenPosition, ToggleDuration).setOnComplete(() =>
        {
            if (DisableWhenOffScreen)
            {
                gameObject.SetActive(false);
            }
            else if (FadeOutWhenOffScreen && BackgroundImage != null && IsVisible)
            {
                TweenBGImageAlpha(BackgroundImage.color.a, OffScreenBGImageAlpha);
            }

            // Let the proxy know to show itself again
            if (SlidingPanelProxy != null)
            {
                SlidingPanelProxy.Show();
            }

            mIsTransitioning = false;
            mIsCurrentlyOnScreen = false;
        });
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (Swipeable.CheckForSwipe(eventData, SwipeToCloseData))
        {
            SlideOff();
        }
        else if (Swipeable.CheckForSwipe(eventData, SwipeToOpenData))
        {
            SlideOn();
        }
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Does nothing, here so OnPointerUp will fire
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TriggerOnClick && !eventData.dragging)
        {
            SlideOn();
        }
    }

    // -------------------------------------------------------------------------------

    public override void OnPointerEnter(PointerEventData data)
    {
        if (mIsVisible && mIsEnabled)
        {
            TweenBGImageAlpha(BackgroundImage.color.a, 1.0f);
        }
        base.OnPointerEnter(data);
    }

    // -------------------------------------------------------------------------------

    public override void OnPointerExit(PointerEventData data)
    {
        if (!mIsCurrentlyOnScreen && mIsVisible && mIsEnabled && !mIsTransitioning)
        {
            TweenBGImageAlpha(BackgroundImage.color.a, OffScreenBGImageAlpha);
        }        
        base.OnPointerExit(data);
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        if(BackgroundImage != null)
        {
            TweenBGImageAlpha(BackgroundImage.color.a, mIsCurrentlyOnScreen ? 1.0f : OffScreenBGImageAlpha);
            IsVisible = true;
        }
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (BackgroundImage != null)
        {
            TweenBGImageAlpha(BackgroundImage.color.a, 0.0f);
        }

        if (IsOnScreen)
        {
            SlideOff();
        }

        IsVisible = false;
    }

    // -------------------------------------------------------------------------------

    private void TweenBGImageAlpha(float from, float to)
    {
        if (mAlphaTweenId != -1)
        {
            LeanTween.cancel(mAlphaTweenId);
        }

        mAlphaTweenId = LeanTween.value(from, to, ToggleDuration).setOnUpdate((float alpha) =>
        {
            var newColor = BackgroundImage.color;
            newColor.a = alpha;
            BackgroundImage.color = newColor;
        }).id;
    }

    // -------------------------------------------------------------------------------
}
