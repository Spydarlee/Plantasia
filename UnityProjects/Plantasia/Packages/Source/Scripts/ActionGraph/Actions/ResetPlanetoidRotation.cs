using ActionGraph;

public class ResetPlanetoidRotation : Action
{
    // -------------------------------------------------------------------------------

    public Variable<Planetoid>  TargetPlanetoid = null;
    public bool                 WaitUntilComplete = false;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (WaitUntilComplete)
        {
            TargetPlanetoid.Value.ResetRotation(() => { FinishAction(); });
        }
        else
        {
            TargetPlanetoid.Value.ResetRotation();
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------
}
