using ActionGraph;

public class SpawnFirstPlant : Action
{
    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        TutorialManager.Instance.SpawnFirstPlant();
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
