using ActionGraph;
using UnityEngine;

public class LeanTweenBaseAction : Action
{
    // -------------------------------------------------------------------------------

    public LeanTweenType    TweenType = LeanTweenType.linear;
    public float            Duration = 1.0f;
    public bool             WaitUntilComplete = true;

    // -------------------------------------------------------------------------------

    protected LTDescr       mTweenDescriptor = null;

    // -------------------------------------------------------------------------------

    protected void FinishExecuting()
    {
        if (mTweenDescriptor == null)
        {
            Debug.LogError("Can't set tween type because the descriptor is null! What's up?");
            FinishAction();
            return;
        }

        SetTweenType();

        if (WaitUntilComplete)
        {
            mTweenDescriptor.setOnComplete(() =>
            {
                FinishAction();
            });
        }
        else
        {
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------

    private void SetTweenType()
    {
        switch (TweenType)
        {
            case LeanTweenType.linear:              mTweenDescriptor.setEaseLinear(); break;
            case LeanTweenType.easeOutQuad:         mTweenDescriptor.setEaseOutQuad(); break;
            case LeanTweenType.easeInQuad:          mTweenDescriptor.setEaseInQuad(); break;
            case LeanTweenType.easeInOutQuad:       mTweenDescriptor.setEaseInOutQuad(); break;
            case LeanTweenType.easeInCubic:         mTweenDescriptor.setEaseInCubic(); break;
            case LeanTweenType.easeOutCubic:        mTweenDescriptor.setEaseOutCubic(); break;
            case LeanTweenType.easeInOutCubic:      mTweenDescriptor.setEaseInOutCubic(); break;
            case LeanTweenType.easeInQuart:         mTweenDescriptor.setEaseInQuart(); break;
            case LeanTweenType.easeOutQuart:        mTweenDescriptor.setEaseOutQuart(); break;
            case LeanTweenType.easeInOutQuart:      mTweenDescriptor.setEaseInOutQuart(); break;
            case LeanTweenType.easeInQuint:         mTweenDescriptor.setEaseInQuint(); break;
            case LeanTweenType.easeOutQuint:        mTweenDescriptor.setEaseOutQuint(); break;
            case LeanTweenType.easeInOutQuint:      mTweenDescriptor.setEaseInOutQuint(); break;
            case LeanTweenType.easeInSine:          mTweenDescriptor.setEaseInSine(); break;
            case LeanTweenType.easeOutSine:         mTweenDescriptor.setEaseOutSine(); break;
            case LeanTweenType.easeInOutSine:       mTweenDescriptor.setEaseInOutSine(); break;
            case LeanTweenType.easeInExpo:          mTweenDescriptor.setEaseInExpo(); break;
            case LeanTweenType.easeOutExpo:         mTweenDescriptor.setEaseOutExpo(); break;
            case LeanTweenType.easeInOutExpo:       mTweenDescriptor.setEaseInOutExpo(); break;
            case LeanTweenType.easeInCirc:          mTweenDescriptor.setEaseInCirc(); break;
            case LeanTweenType.easeOutCirc:         mTweenDescriptor.setEaseOutCirc(); break;
            case LeanTweenType.easeInOutCirc:       mTweenDescriptor.setEaseInOutCirc(); break;
            case LeanTweenType.easeInBounce:        mTweenDescriptor.setEaseInBounce(); break;
            case LeanTweenType.easeOutBounce:       mTweenDescriptor.setEaseOutBounce(); break;
            case LeanTweenType.easeInOutBounce:     mTweenDescriptor.setEaseInOutBounce();break;
            case LeanTweenType.easeInBack:          mTweenDescriptor.setEaseInBack(); break;
            case LeanTweenType.easeOutBack:         mTweenDescriptor.setEaseOutBack(); break;
            case LeanTweenType.easeInOutBack:       mTweenDescriptor.setEaseInOutBack(); break;
            case LeanTweenType.easeInElastic:       mTweenDescriptor.setEaseInElastic(); break;
            case LeanTweenType.easeOutElastic:      mTweenDescriptor.setEaseOutElastic(); break;
            case LeanTweenType.easeInOutElastic:    mTweenDescriptor.setEaseInOutElastic(); break;
            case LeanTweenType.easeSpring:          mTweenDescriptor.setEaseSpring(); break;
            case LeanTweenType.easeShake:           mTweenDescriptor.setEaseShake(); break;
            case LeanTweenType.punch:               mTweenDescriptor.setEasePunch(); break;
            case LeanTweenType.once:                mTweenDescriptor.setLoopOnce(); break;
            case LeanTweenType.clamp:               mTweenDescriptor.setLoopClamp(); break;
            case LeanTweenType.pingPong:            mTweenDescriptor.setLoopPingPong(); break;
            case LeanTweenType.animationCurve:      Debug.LogError("Can't set AnimationCurve Tween this way..."); break;
            default:                                Debug.LogError("Invalid TweenType specified. Falling back to Linear"); break;
        }
    }

    // -------------------------------------------------------------------------------
}
