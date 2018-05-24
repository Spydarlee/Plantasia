using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // -------------------------------------------------------------------------------

    public bool     HideOnStart = false;
    public float    ToggleDuration = 0.25f;
    public string   HideSfx = AudioManager.InvalidSfxName;
    public string   ShowSfx = AudioManager.InvalidSfxName;

    // -------------------------------------------------------------------------------

    public delegate void UIElementShowBeginAction();
    public event UIElementShowBeginAction OnShowBegin = null;

    public delegate void UIElementHideBeginAction();
    public event UIElementHideBeginAction OnHideBegin = null;

    // -------------------------------------------------------------------------------

    protected bool  mIsVisible = true;
    protected bool  mIsUnderPointer = false;
    protected bool  mIsEnabled = true;

    // -------------------------------------------------------------------------------

    public bool IsVisible
    {
        get { return mIsVisible; }
        protected set { mIsVisible = value; }
    }

    // -------------------------------------------------------------------------------

    public bool IsEnabled
    {
        get { return mIsEnabled; }
        set { SetDisabled(value); }
    }

    // -------------------------------------------------------------------------------

    protected virtual void Start()
    {
        if (HideOnStart)
        {
            Hide(true);
        }
    }

    // -------------------------------------------------------------------------------

    public virtual void OnPointerEnter(PointerEventData data)
    {
        if (mIsVisible && mIsEnabled)
        {
            UIManager.Instance.NotifyPointerEntered(this);
            mIsUnderPointer = true;
        }
    }

    // -------------------------------------------------------------------------------

    public virtual void OnPointerExit(PointerEventData data)
    {
        UIManager.Instance.NotifyPointerExited(this);
        mIsUnderPointer = false;
    }

    // -------------------------------------------------------------------------------

    public void Show(bool instant = false)
    {
        if (!mIsVisible && mIsEnabled)
        {
            if (OnShowBegin != null)
            {
                OnShowBegin.Invoke();
            }

            if (!instant && ShowSfx != AudioManager.InvalidSfxName)
            {
                AudioManager.Instance.PlaySfx(null, ShowSfx);
            }

            OnShow();
        }
    }

    // -------------------------------------------------------------------------------

    protected virtual void OnShow(bool instant = false)
    {
        mIsVisible = true;

        if (!gameObject.activeSelf)
        {
            if (instant)
            {
                gameObject.SetActive(true);
                transform.localScale = Vector3.one;
            }
            else
            {
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                LeanTween.scale(gameObject, Vector3.one, ToggleDuration).setEaseInSine();
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void Hide(bool instant = false)
    {
        if (mIsVisible)
        {
            if (OnHideBegin != null)
            {
                OnHideBegin.Invoke();
            }

            UIManager.Instance.NotifyPointerExited(this);

            if (!instant && HideSfx != AudioManager.InvalidSfxName)
            {
                AudioManager.Instance.PlaySfx(null, HideSfx);
            }

            OnHide(instant);
        }
    }

    // -------------------------------------------------------------------------------

    protected virtual void OnHide(bool instant = false)
    {
        IsVisible = false;

        if (gameObject.activeSelf)
        {
            if (instant)
            {
                transform.localScale = Vector3.zero;
                OnHideComplete();
            }
            else
            {
                LeanTween.scale(gameObject, Vector3.zero, ToggleDuration).setEaseOutSine().setOnComplete(() =>
               {
                   OnHideComplete();
               });
            }
        }           
    }

    // -------------------------------------------------------------------------------

    protected virtual void OnHideComplete()
    {
        gameObject.SetActive(false);
        mIsVisible = false;
    }

    // -------------------------------------------------------------------------------

    private void SetDisabled(bool value)
    {
        mIsEnabled = value;

        if (!mIsEnabled && mIsVisible)
        {
            Hide(true);
        }

        mIsUnderPointer = false;
    }

    // -------------------------------------------------------------------------------
}
