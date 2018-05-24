using UnityEngine.UI;

public class MessageBoxUI : UIElement
{
    // -------------------------------------------------------------------------------

    public Text         TitleText = null;
    public Text         MessageText = null;
    public UIElement    ScreenFill = null;

    // -------------------------------------------------------------------------------

    public delegate void MessageBoxClosedAction();
    public static event MessageBoxClosedAction OnMessageBoxClosed = null;

    // -------------------------------------------------------------------------------

    private bool mNeedToShowSeedSelectionUI = false;

    // -------------------------------------------------------------------------------

    public void Show(string titleTextLocID, string messageTextLocId, bool instant = false)
    {
        TitleText.text = (titleTextLocID != "") ? LocalisationManager.Instance.GetLocText(titleTextLocID) : "";
        MessageText.text = LocalisationManager.Instance.GetLocText(messageTextLocId);
        Show(instant);
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        if (UIManager.Instance.SeedSelectionUI.IsVisible)
        {
            mNeedToShowSeedSelectionUI = true;
            UIManager.Instance.SeedSelectionUI.Hide();
        }

        if (ScreenFill != null)
        {
            ScreenFill.Show(instant);
        }

        Universe.Instance.CurrentPlanetoid.IsPaused = true;
        base.OnShow(instant);
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (ScreenFill != null)
        {
            ScreenFill.Hide(instant);
        }

        base.OnHide(instant);
    }

    // -------------------------------------------------------------------------------

    protected override void OnHideComplete()
    {
        if (OnMessageBoxClosed != null)
        {
            OnMessageBoxClosed.Invoke();
        }

        if (mNeedToShowSeedSelectionUI)
        {
            mNeedToShowSeedSelectionUI = false;
            UIManager.Instance.SeedSelectionUI.Show();
        }

        Universe.Instance.CurrentPlanetoid.IsPaused = false;

        base.OnHideComplete();
    }

    // -------------------------------------------------------------------------------
}
