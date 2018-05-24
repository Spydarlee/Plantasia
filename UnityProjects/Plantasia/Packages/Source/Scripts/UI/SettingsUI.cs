public class SettingsUI : UIElement
{
    // -------------------------------------------------------------------------------

    public UIElement Background = null;
    public UIElement CloseSettingsButton = null;

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        Background.Show();
        CloseSettingsButton.Show();

        base.OnShow(instant);
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        Background.Hide();
        CloseSettingsButton.Hide();

        base.OnHide(instant);
    }

    // -------------------------------------------------------------------------------
}
