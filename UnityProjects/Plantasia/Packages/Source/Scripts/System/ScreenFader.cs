using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static ScreenFader   Instance = null;
    public Image                BlackScreenImage = null;
    public float                FadeDuration = 1.0f;
    public bool                 BlackToFadeOnStart = false;

    // -------------------------------------------------------------------------------

    public bool                 IsFading { get { return mIsFading; } }

    // -------------------------------------------------------------------------------

    private bool                mIsFading = false;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (BlackScreenImage == null)
        {
            BlackScreenImage = GetComponent<Image>();
        }
	}

    // ------------------------------------------------------------------------------- 

    private void Start()
    {
        if (BlackToFadeOnStart)
        {
            BlackToFade();
        }
    }

    // ------------------------------------------------------------------------------- 

    public void FadeToBlack(Action onCompleteCallback = null)
    {
        if (!mIsFading)
        {
            mIsFading = true;

            var startColor = Color.black;
            startColor.a = 0.0f;
            BlackScreenImage.color = startColor;

            LeanTween.value(0.0f, 1.0f, FadeDuration).setOnUpdate((float value) =>
            {
                var newColor = BlackScreenImage.color;
                newColor.a = value;
                BlackScreenImage.color = newColor;

            }).setOnComplete(()=>
            {
                if (onCompleteCallback != null)
                {
                    onCompleteCallback.Invoke();
                }

                mIsFading = false;
            });
        }
    }

    // ------------------------------------------------------------------------------- 

    public void BlackToFade(Action onCompleteCallback = null)
    {
        if (!mIsFading)
        {
            mIsFading = true;

            var startColor = Color.black;
            startColor.a = 1.0f;
            BlackScreenImage.color = startColor;

            LeanTween.value(1.0f, 0.0f, FadeDuration).setOnUpdate((float value) =>
            {
                var newColor = BlackScreenImage.color;
                newColor.a = value;
                BlackScreenImage.color = newColor;

            }).setOnComplete(() =>
            {
                if (onCompleteCallback != null)
                {
                    onCompleteCallback.Invoke();
                }

                mIsFading = false;
            });
        }
    }

    // -------------------------------------------------------------------------------
}
