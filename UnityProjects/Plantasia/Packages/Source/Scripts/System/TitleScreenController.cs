using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public string   SceneToLoad = "Main";
    public float    MaxDistForTouchNotDrag = 1.0f;

    private Vector3 mMouseDownPos = Vector3.zero;
    private bool    mIsLoadingMainGame = false;

    // -------------------------------------------------------------------------------

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        AudioManager.Instance.PlayBGMusic("TitleScreen");
    }

    // -------------------------------------------------------------------------------

    void Update()
    {
        if (!mIsLoadingMainGame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mMouseDownPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0) && !ScreenFader.Instance.IsFading)
            {
                // Did the user click/tap without deagging?
                if (Vector3.Distance(mMouseDownPos, Input.mousePosition) <= MaxDistForTouchNotDrag)
                {
                    mIsLoadingMainGame = true;
                    ScreenFader.Instance.FadeToBlack(OnFadedToBlack);
                    AudioManager.Instance.StopBGMusic();
                }
            }
        }
	}

    // -------------------------------------------------------------------------------

    private void OnFadedToBlack()
    {
        SceneManager.LoadScene(SceneToLoad);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // -------------------------------------------------------------------------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScreenFader.Instance.BlackToFade();
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // We're not on the title screen anymore so we can get going now
        GameObject.Destroy(gameObject);
    }

    // -------------------------------------------------------------------------------
}
