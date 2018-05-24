using ActionGraph;

public class CheckTutorialStage : Condition
{
    // -------------------------------------------------------------------------------

    public TutorialStages StageToMatch;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return TutorialManager.Instance.CurrentStage == StageToMatch;
    }

    // -------------------------------------------------------------------------------
}
