using ActionGraph;

public class PlantIsFullyGrown : Condition
{
    // -------------------------------------------------------------------------------

    public Variable<Plant> Plant;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return (Plant == null || Plant.Value == null || Plant.Value.IsFullyGrown);
    }

    // -------------------------------------------------------------------------------
}
