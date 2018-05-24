using ActionGraph;

public class CheckPlantCount : Condition
{
    // -------------------------------------------------------------------------------

    public int PlantCount = 2;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return Universe.Instance.CurrentPlanetoid.PlantCount >= PlantCount;
    }

    // -------------------------------------------------------------------------------
}
