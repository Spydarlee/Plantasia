using ActionGraph;

public class SetOrbitableAngle : Action
{
    // -------------------------------------------------------------------------------

    public Variable<Orbitable>  OrbitableTarget = null;
    public float                Angle = 0.0f;
    public float                Duration = 3.0f;
    public bool                 PauseOnComplete = true;
    public bool                 WaitUntilComplete = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Set " + OrbitableTarget + " angle to " + Angle; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (WaitUntilComplete)
        {
            OrbitableTarget.Value.SetAngle(Angle, Duration, PauseOnComplete, () =>
            {
                FinishAction();
            });
        }
        else
        {
            OrbitableTarget.Value.SetAngle(Angle, Duration, PauseOnComplete);
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------
}