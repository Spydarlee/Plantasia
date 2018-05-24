public class BuriedPlanetoidProxy : BuriedObject
{
    // -------------------------------------------------------------------------------

    private PlanetoidProxy mPlanetoidProxy;

    // -------------------------------------------------------------------------------

    public Planetoid.PlanetoidTypes PlanetoidType { get { return mPlanetoidProxy.Planetoid.PlanetoidType; } }

    // -------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();

        mPlanetoidProxy = ObjectToBury.GetComponent<PlanetoidProxy>();
    }

    // -------------------------------------------------------------------------------

    protected override void Initialise()
    {
        base.Initialise();

        // If the proxy is being controlled by us we don't want it to be active
        // or respond to any user input until it's fully unearthed!
        mPlanetoidProxy.SetActive(false);
    }

    // -------------------------------------------------------------------------------

    protected override void OnUnburied()
    {
        mPlanetoidProxy.transform.parent = null;
        SaveGameManager.Instance.UnlockedHomePlanetoid(mPlanetoidProxy.Planetoid.PlanetoidType);
        base.OnUnburied();

        FoundBuriedObjectState.FoundPlanetoidProxy = mPlanetoidProxy;   // KB TODO
        GameManager.Instance.ChangeGameplayState(GameplayStates.FoundBuriedObject);
    }

    // -------------------------------------------------------------------------------
}
