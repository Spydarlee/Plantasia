using UnityEngine.UI;
using UnityEngine;

public class ZoomPrompt : UIElement
{
    // -------------------------------------------------------------------------------

    public Image LeftHand = null;
    public Image RightHand = null;
    public float Distance = 50.0f;
    public float TweenDuration = 0.5f;

    // -------------------------------------------------------------------------------

    private Vector2         mLeftHandDefaultPos = Vector2.zero;
    private Vector2         mRightHandDefaultPos = Vector2.zero;
    private int             mLeftHandTweenId = -1;
    private int             mRightHandTweenId = -1;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        mLeftHandDefaultPos = LeftHand.rectTransform.anchoredPosition;
        mRightHandDefaultPos = RightHand.rectTransform.anchoredPosition;
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        Vector2 leftHandTargetPos = LeftHand.rectTransform.anchoredPosition;
        leftHandTargetPos.x -= Distance;
        LeftHand.rectTransform.anchoredPosition = mLeftHandDefaultPos;
        mLeftHandTweenId = LeanTween.move(LeftHand.rectTransform, leftHandTargetPos, TweenDuration).setLoopPingPong().id;

        Vector2 rightHandTargetPos = RightHand.rectTransform.anchoredPosition;
        rightHandTargetPos.x += Distance;
        RightHand.rectTransform.anchoredPosition = mRightHandDefaultPos;
        mRightHandTweenId = LeanTween.move(RightHand.rectTransform, rightHandTargetPos, TweenDuration).setLoopPingPong().id;

        base.OnShow(instant);
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (mLeftHandTweenId != -1)
        {
            LeanTween.cancel(mLeftHandTweenId);
            LeanTween.cancel(mRightHandTweenId);

            mLeftHandTweenId = mRightHandTweenId = -1;
        }

        base.OnHide(instant);
    }

    // -------------------------------------------------------------------------------
}
