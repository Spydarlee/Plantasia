using UnityEngine;
using UnityEngine.UI;

public class DragPromptUI : UIElement
{
    // -------------------------------------------------------------------------------

    public Image            HandImage = null;
    public float            TweenDuration = 0.5f;

    // -------------------------------------------------------------------------------

    private RectTransform   mRectTransform = null;
    private GameObject      mWorldSpaceTarget = null;
    private Vector3         mWorldSpaceTargetOffset = Vector3.zero;
    private bool            mTargetIsInWorldSpace = true;
    private int             mTweenID = -1;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        mRectTransform = transform as RectTransform;
    }

    // -------------------------------------------------------------------------------

    public void ShowAtWorldTarget(GameObject target, Vector3 offset, Vector2 dragScreenDirection, float dragScreenDistance)
    {
        LeanTween.cancel(gameObject);
        mWorldSpaceTarget = target;
        mWorldSpaceTargetOffset = offset;
        mTargetIsInWorldSpace = true;
        StartImageTween(dragScreenDirection, dragScreenDistance);
        base.Show();
    }

    // -------------------------------------------------------------------------------

    public void ShowAtScreenPosition(Vector2 screenPosition, Vector2 dragScreenDirection, float dragScreenDistance)
    {
        LeanTween.cancel(gameObject);
        mRectTransform.anchoredPosition = screenPosition;
        mTargetIsInWorldSpace = false;
        StartImageTween(dragScreenDirection, dragScreenDistance);
        base.Show();
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (mTweenID != -1)
        {
            LeanTween.cancel(mTweenID);
            mTweenID = -1;
        }

        base.OnHide(instant);
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        if (mIsVisible)
        {
            if (mTargetIsInWorldSpace)
            {
                var targetWorldPosition = (mWorldSpaceTarget != null) ? mWorldSpaceTarget.transform.position : Vector3.zero;
                targetWorldPosition += mWorldSpaceTargetOffset;
                mRectTransform.position = Camera.main.WorldToScreenPoint(targetWorldPosition);
            }
        }
    }

    // -------------------------------------------------------------------------------

    private void StartImageTween(Vector2 direction, float distance)
    {
        mTweenID = LeanTween.move(HandImage.rectTransform, (direction * distance), TweenDuration).setOnComplete(() =>
        {
            if (mIsVisible && mIsEnabled)
            {
                LeanTween.delayedCall(0.5f, () => { HandImage.rectTransform.anchoredPosition = Vector2.zero; StartImageTween(direction, distance); });
            }           

        }).id;
    }

    // -------------------------------------------------------------------------------
}
