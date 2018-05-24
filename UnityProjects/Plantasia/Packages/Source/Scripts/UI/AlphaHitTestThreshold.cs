using UnityEngine;
using UnityEngine.UI;

public class AlphaHitTestThreshold : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public Image    TargetImage = null;
    public float    AlphaHitTestMinThreshold = 0.5f;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (TargetImage == null)
        {
            TargetImage = GetComponent<Image>();
        }

        if (TargetImage != null)
        {
            TargetImage.alphaHitTestMinimumThreshold = AlphaHitTestMinThreshold;
        }
    }

    // -------------------------------------------------------------------------------
}
