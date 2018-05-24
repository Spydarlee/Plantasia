using ActionGraph;

public class ChoosingAStarState : Action
{
    // -------------------------------------------------------------------------------

    private GameManager mGameManager = null;
    private Universe    mUniverse = null;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mUniverse = mGameManager.Universe;

        mUniverse.OnPlanetoidChange += OnPlanetoidChange;
        mUniverse.ShowDistantStars();
    }

    // -------------------------------------------------------------------------------

    protected override void OnFinish()
    {
        mUniverse.HideDistantStars();
        mUniverse.OnPlanetoidChange -= OnPlanetoidChange;
    }

    // -------------------------------------------------------------------------------

    private void OnPlanetoidChange(Planetoid newPlanetoid)
    {
        mGameManager.ChangeGameplayState(GameplayStates.RidingSunbeams);
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}