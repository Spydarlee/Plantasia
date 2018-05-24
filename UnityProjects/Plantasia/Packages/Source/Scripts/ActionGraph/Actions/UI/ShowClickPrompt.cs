using ActionGraph;
using UnityEngine;

public class ShowClickPrompt : Action
{
    // -------------------------------------------------------------------------------

    public Variable<GameObject> TargetObject = null;
    public Vector3              TargetOffset = Vector3.zero;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        UIManager.Instance.ClickPromptUI.ShowAtWorldTarget(TargetObject.Value, TargetOffset);
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
