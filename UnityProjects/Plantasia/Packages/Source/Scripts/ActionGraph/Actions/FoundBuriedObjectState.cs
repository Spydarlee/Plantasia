using UnityEngine;
using ActionGraph;

public class FoundBuriedObjectState : Action
{
    // -------------------------------------------------------------------------------

    // Bit of a hack.. could use a nicer way to pass this into the state!
    public static PlanetoidProxy FoundPlanetoidProxy = null;

    // -------------------------------------------------------------------------------

    public float OffsetFromCamera = 1.75f;
    public float MinDuration = 2.0f;

    // -------------------------------------------------------------------------------

    private GameManager mGameManager = null;
    private UIManager mUIManager = null;

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        mGameManager = GameManager.Instance;
        mGameManager.CameraController.DisableZoomInput = true;
        mGameManager.CurrentPlanetoid.IsPaused = true;

        LeanTween.rotate(FoundPlanetoidProxy.gameObject, Vector3.zero, 1.0f);

        var offsetDirection = (Vector3.zero - mGameManager.CameraController.MainCamera.transform.position).normalized;
        Vector3 targetPosition = mGameManager.CameraController.transform.position + offsetDirection * OffsetFromCamera;
        LeanTween.move(FoundPlanetoidProxy.gameObject, targetPosition, 1.0f).setOnComplete(() =>
        {
            FoundPlanetoidProxy.SetActive(true);
        });

        FoundPlanetoidProxy.OnProxyClicked += OnProxyClicked;
        mUIManager = UIManager.Instance;
        mUIManager.FoundBuriedItemUI.Show();
    }

    // -------------------------------------------------------------------------------

    private void OnProxyClicked()
    {
        if (mElapsedTime >= MinDuration)
        {
            mUIManager.FoundBuriedItemUI.Hide();
            FoundPlanetoidProxy.OnProxyClicked -= OnProxyClicked;

            Vector3 originalProxyScale = FoundPlanetoidProxy.transform.localScale;
            LeanTween.scale(FoundPlanetoidProxy.gameObject, Vector2.zero, 1.0f).setOnComplete(() =>
            {
                FoundPlanetoidProxy.gameObject.SetActive(false);
                FoundPlanetoidProxy.transform.localScale = originalProxyScale;

                mGameManager.ChangeGameplayState(GameplayStates.Planetoid);
                mGameManager.CurrentPlanetoid.IsPaused = false;
                mGameManager.CameraController.DisableZoomInput = false;

                FinishAction();
            });
        }
    }

    // -------------------------------------------------------------------------------
}