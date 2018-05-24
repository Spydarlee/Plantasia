using UnityEngine;

public class ClickPromptUI : UIElement
{
    // -------------------------------------------------------------------------------

    private RectTransform   mRectTransform = null;
    private GameObject      mTarget = null;
    private Vector3         mTargetOffset = Vector3.zero;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        mRectTransform = transform as RectTransform;
    }

    // -------------------------------------------------------------------------------

    public void ShowAtWorldTarget(GameObject target, Vector3 offset)
    {
        mTarget = target;
        mTargetOffset = offset;
        base.Show();
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        if (mIsVisible)
        {
            var targetWorldPosition = (mTarget != null) ? mTarget.transform.position : Vector3.zero;
            targetWorldPosition += mTargetOffset;
            mRectTransform.position = Camera.main.WorldToScreenPoint(targetWorldPosition);
        }
    }

    // -------------------------------------------------------------------------------
}
