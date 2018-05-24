using UnityEngine.UI;

public class ImageUIElement : UIElement
{
    // -------------------------------------------------------------------------------

    public Image Image = null;

    // -------------------------------------------------------------------------------

    private float m_MaxAlpha = 1.0f;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (Image == null)
        {
            Image = GetComponent<Image>();
        }

        m_MaxAlpha = Image.color.a;
    }

    // -------------------------------------------------------------------------------

    protected override void OnShow(bool instant = false)
    {
        gameObject.SetActive(true);

        if (instant)
        {
            SetAlpha(m_MaxAlpha);
        }
        else
        {
            TweenAlpha(Image.color.a, m_MaxAlpha);
        }

        IsVisible = true;
    }

    // -------------------------------------------------------------------------------

    protected override void OnHide(bool instant = false)
    {
        if (instant)
        {
            SetAlpha(0.0f);
            gameObject.SetActive(false);
        }
        else
        {
            TweenAlpha(Image.color.a, 0.0f);
        }

        IsVisible = false;
    }

    // -------------------------------------------------------------------------------

    private void TweenAlpha(float from, float to)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(from, to, ToggleDuration).setOnUpdate((float value) =>
        {
            var newColor = Image.color;
            newColor.a = value;
            Image.color = newColor;

        }).setOnComplete(()=>
        {
            if (to == 0.0f)
            {
                gameObject.SetActive(false);
            }            
        });
    }

    // -------------------------------------------------------------------------------

    private void SetAlpha(float alpha)
    {
        var newColor = Image.color;
        newColor.a = alpha;
        Image.color = newColor;
    }

    // -------------------------------------------------------------------------------
}