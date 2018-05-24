using ActionGraph;

public class PlaySFX : Action
{
    // -------------------------------------------------------------------------------

    public string SFXName = "";

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Play SFX: " + SFXName; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        AudioManager.Instance.PlaySfx(null, SFXName);
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
