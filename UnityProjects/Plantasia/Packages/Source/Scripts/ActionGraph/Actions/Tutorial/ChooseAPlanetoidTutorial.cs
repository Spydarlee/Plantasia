using ActionGraph;
using UnityEngine;

public class ChooseAPlanetoidTutorial : Action
{
    // -------------------------------------------------------------------------------

    public Vector2  DragPromptScreenPos = new Vector2(300, 300);
    public Vector2  DragPromptScreenDir = new Vector2(1, 0);
    public float    DragPromptScreenDist = 100.0f;
    public float    TimeWithoutInputToShowDragPrompt = 2.0f;

    // -------------------------------------------------------------------------------

    private GameManager mGameManager = null;
    private Universe    mUniverse = null;
    private Rotatable   mUniverseSphereRotatable = null;
    private UIManager   mUIManager = null;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mUniverse = Universe.Instance;
        mUniverseSphereRotatable = mUniverse.UniverseSphere.GetComponent<Rotatable>();
        mUIManager = UIManager.Instance;

        mUIManager.DragPromptUI.ShowAtScreenPosition(DragPromptScreenPos, DragPromptScreenDir, DragPromptScreenDist);
    }

    // -------------------------------------------------------------------------------

    protected override void OnUpdate()
    {
        switch (mGameManager.CurrentGameplayState)
        {
            case GameplayStates.Planetoid:
                {
                    if (mUniverse.CurrentPlanetoid != mUniverse.CurrentHomePlanetoid &&
                        mGameManager.IsTransitioning == false)
                    {
                        FinishAction();
                    }
                }
                break;

            case GameplayStates.ChoosingAStar:
                {
                    var timeSinceUniverseSphereInput = mUniverseSphereRotatable.TimeSinceInput;

                    if (!mUIManager.DragPromptUI.IsVisible && timeSinceUniverseSphereInput >= TimeWithoutInputToShowDragPrompt)
                    {
                        mUIManager.DragPromptUI.ShowAtScreenPosition(DragPromptScreenPos, DragPromptScreenDir, DragPromptScreenDist);
                    }
                    else if (mUIManager.DragPromptUI.IsVisible && timeSinceUniverseSphereInput < TimeWithoutInputToShowDragPrompt)
                    {
                        mUIManager.DragPromptUI.Hide();
                    }
                }
                break;

            default:
                {
                    if (mUIManager.DragPromptUI.IsVisible)
                    {
                        mUIManager.DragPromptUI.Hide();
                    }                    
                }
                break;
        }
    }

    // -------------------------------------------------------------------------------
}
