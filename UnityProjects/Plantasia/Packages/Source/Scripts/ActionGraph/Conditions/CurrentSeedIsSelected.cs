using ActionGraph;

public class CurrentSeedIsSelected : Condition
{
    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return GameManager.Instance.SeedShip.CurrentSeed != null;
    }

    // -------------------------------------------------------------------------------
}