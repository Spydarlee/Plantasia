using ActionGraph;
using UnityEngine;

public class LeanTweenScale : LeanTweenBaseAction
{
    // -------------------------------------------------------------------------------

    public Variable<GameObject> Target;
    public Vector3              Scale;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Tween Scale " + Target; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mTweenDescriptor = LeanTween.scale(Target.Value, Scale, Duration);
        FinishExecuting();
    }

    // -------------------------------------------------------------------------------
}
