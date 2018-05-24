using ActionGraph;

public class SetFlipableEnabled : Action
{
    // -------------------------------------------------------------------------------

    public Variable<Flipable>   Flipable;
    public bool                 Enabled = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName
    {
        get { return (Enabled) ? "Enable " + Flipable + " Flipable" : "Disable " + Flipable + " Flipable"; }
    }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (Flipable != null && Flipable.Value != null)
        {
            Flipable.Value.enabled = Enabled;
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------
}
