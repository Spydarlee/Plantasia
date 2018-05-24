using ActionGraph;

public class SetOrbitablePaused : Action
{
    // -------------------------------------------------------------------------------

    public Variable<Orbitable>  OrbitableTarget = null;
    public bool                 Paused = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName
    {
        get { return (Paused) ? ("Pause " + OrbitableTarget) : ("Unpause " + OrbitableTarget); }
    }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        OrbitableTarget.Value.IsPaused = Paused;
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
