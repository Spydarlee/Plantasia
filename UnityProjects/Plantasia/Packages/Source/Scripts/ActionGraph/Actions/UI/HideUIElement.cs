using ActionGraph;

public class HideUIElement : Action
{
    // -------------------------------------------------------------------------------

    public Variable<UIElement> UIElement;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Hide " + UIElement; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (UIElement != null && UIElement.Value != null)
        {
            UIElement.Value.Hide();
        }

        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
