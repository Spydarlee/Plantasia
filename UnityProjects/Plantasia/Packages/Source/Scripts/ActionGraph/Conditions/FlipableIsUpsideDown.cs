using ActionGraph;

public class FlipableIsUpsideDown : Condition
{
    // -------------------------------------------------------------------------------

    public Variable<Flipable> Flipable;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        return (Flipable != null && Flipable.Value != null && Flipable.Value.IsUpsideDown);
    }

    // -------------------------------------------------------------------------------
}