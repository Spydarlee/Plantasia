using ActionGraph;
using UnityEngine;

public class ShowDragPrompt : Action
{
    // -------------------------------------------------------------------------------

    public Variable<GameObject> WorldSpaceTarget = null;
    public Vector3              WorldSpaceTargetOffset = Vector3.zero;
    public Vector2              ScreenSpaceTarget = Vector2.zero;
    public bool                 UseScreenSpaceTarget = false;
    public Vector2              ScreenDirection = Vector2.right;
    public float                ScreenDistance = 50.0f;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (UseScreenSpaceTarget)
        {
            UIManager.Instance.DragPromptUI.ShowAtScreenPosition(ScreenSpaceTarget, ScreenDirection, ScreenDistance);
        }
        else
        {
            UIManager.Instance.DragPromptUI.ShowAtWorldTarget(WorldSpaceTarget.Value, WorldSpaceTargetOffset, ScreenDirection, ScreenDistance);
        }
        
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
