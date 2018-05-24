using ActionGraph;

public class SetTutorialStage : Action
{
    // -------------------------------------------------------------------------------

    public TutorialStages TutorialStage;

    // -------------------------------------------------------------------------------

    public override string DisplayName
    {
        get
        {
            return "Set Tutorial Stage: " + TutorialStage.ToString();
        }
    }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        TutorialManager.Instance.CurrentStage = TutorialStage;
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
