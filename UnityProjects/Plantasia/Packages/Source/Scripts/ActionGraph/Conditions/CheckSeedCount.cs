using ActionGraph;

public class CheckSeedCount : Condition
{
    // -------------------------------------------------------------------------------

    public int NumberToCheck = 1;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        if (GameManager.Instance.SeedShip.GetSeedCount() >= NumberToCheck)
        {
            return true;
        }

        return false;
    }

    // -------------------------------------------------------------------------------
}
