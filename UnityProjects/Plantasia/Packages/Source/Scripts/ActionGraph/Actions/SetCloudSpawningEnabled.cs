using ActionGraph;

public class SetCloudSpawningEnabled : Action
{
    // -------------------------------------------------------------------------------

    public bool Enabled = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return (Enabled) ? "Enable Cloud Spawning" : "Disable Cloud Spawning"; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        Universe.Instance.CloudSpawningEnabled = Enabled;
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
