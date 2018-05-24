using ActionGraph;

public class SetUIElementEnabled : Action
{
    // -------------------------------------------------------------------------------

    public Variable<UIElement>  UIElement;
    public bool                 Enabled = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName
    {
        get { return (Enabled) ? "Enable " + UIElement : "Disable " + UIElement; }
    }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (UIElement != null && UIElement.Value != null)
        {
            UIElement.Value.IsEnabled = Enabled;
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------
}
