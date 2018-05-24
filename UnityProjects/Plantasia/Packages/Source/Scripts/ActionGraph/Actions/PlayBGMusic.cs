using ActionGraph;

public class PlayBGMusic : Action
{
    // -------------------------------------------------------------------------------

    public string BGMusicName = "";

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Play BGMusic" + BGMusicName; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        AudioManager.Instance.PlayBGMusic(BGMusicName);
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
