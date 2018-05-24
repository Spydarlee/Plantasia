using ActionGraph;

public class ShowUIElement : Action
{
    // -------------------------------------------------------------------------------

    public Variable<UIElement>  UIElement;
    public bool                 Instant = false;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Show " + UIElement; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (UIElement != null && UIElement.Value != null)
        {
            UIElement.Value.Show(Instant);
        }

        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
