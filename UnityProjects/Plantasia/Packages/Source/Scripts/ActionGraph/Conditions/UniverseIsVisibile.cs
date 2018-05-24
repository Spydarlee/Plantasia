using ActionGraph;

public class UniverseIsVisibile : Condition
{
    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return Universe.Instance.UniverseSphere.IsVisible;
    }

    // -------------------------------------------------------------------------------
}
