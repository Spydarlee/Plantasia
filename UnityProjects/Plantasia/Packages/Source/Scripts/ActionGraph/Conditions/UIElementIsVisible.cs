using ActionGraph;

public class UIElementIsVisible : Condition
{
    // -------------------------------------------------------------------------------

    public Variable<UIElement>  UIElement;
    public bool                 CheckForVisibility = true;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        if (UIElement != null && UIElement.Value != null)
        {
            return UIElement.Value.IsVisible == CheckForVisibility;
        }

        return false;
    }

    // -------------------------------------------------------------------------------
}
