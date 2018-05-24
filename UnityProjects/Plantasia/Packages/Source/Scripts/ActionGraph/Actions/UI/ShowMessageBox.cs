using ActionGraph;

public class ShowMessageBox : Action
{
    // -------------------------------------------------------------------------------

    public string   TitleLocId = LocalisationManager.kInvalidLocID;
    public string   MessageLocId = LocalisationManager.kInvalidLocID;
    public bool     WaitUntilClosed = true;

    // -------------------------------------------------------------------------------

    public override string DisplayName { get { return "Message Box: " + MessageLocId; } }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        UIManager.Instance.MessageBoxUI.Show(TitleLocId, MessageLocId, false);

        if (WaitUntilClosed)
        {
            MessageBoxUI.OnMessageBoxClosed += OnMessageBoxClosed;
        }
        else
        {
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------

    protected virtual void OnMessageBoxClosed()
    {
        MessageBoxUI.OnMessageBoxClosed -= OnMessageBoxClosed;
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
