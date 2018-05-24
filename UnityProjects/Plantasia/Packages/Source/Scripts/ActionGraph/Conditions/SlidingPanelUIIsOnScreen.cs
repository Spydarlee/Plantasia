using ActionGraph;

public class SlidingPanelUIIsOnScreen : Condition
{
    // -------------------------------------------------------------------------------

    public Variable<SlidingPanelUI> SlidingPanel;
    public bool                     CheckForOnScreen = true;

    // -------------------------------------------------------------------------------

    public override bool Check()
    {
        if (SlidingPanel != null && SlidingPanel.Value != null)
        {
            return SlidingPanel.Value.IsOnScreen == CheckForOnScreen;
        }

        return false;
    }

    // -------------------------------------------------------------------------------
}
