using ActionGraph;

public class CheckGameplayState : Condition
{
    // -------------------------------------------------------------------------------

    public GameplayStates StateToMatch;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return GameManager.Instance.CurrentGameplayState == StateToMatch;
    }

    // -------------------------------------------------------------------------------
}
