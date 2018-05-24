using UnityEngine;
using ActionGraph;

public class LeanTweenRotate : LeanTweenBaseAction
{
    // -------------------------------------------------------------------------------

    public Variable<GameObject> Target;
    public Vector3              Rotation;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Tween Rotate " + Target; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mTweenDescriptor = LeanTween.rotate(Target.Value, Rotation, Duration);
        FinishExecuting();
    }

    // -------------------------------------------------------------------------------
}
