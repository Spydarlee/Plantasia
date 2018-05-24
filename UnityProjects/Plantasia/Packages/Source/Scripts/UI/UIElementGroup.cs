using System.Collections.Generic;

public class UIElementGroup : UIElement
{
    // -------------------------------------------------------------------------------

    public List<UIElement> UIElements = new List<UIElement>();

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        foreach (var uiElement in UIElements)
        {
            uiElement.Show(instant);
        }

        IsVisible = true;
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        foreach (var uiElement in UIElements)
        {
            uiElement.Hide(instant);
        }

        IsVisible = false;
    }

    // -------------------------------------------------------------------------------
}
