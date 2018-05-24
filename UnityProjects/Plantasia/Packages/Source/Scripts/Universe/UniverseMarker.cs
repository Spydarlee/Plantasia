using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniverseMarker : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public GameObject       Target = null;

    public float            OnScreenPosOffsetXPercentage = 0.01f;
    public float            OnScreenPosOffsetYPercentage = 0.01f;

    public float            OuterScreenEdgeXPercentage = 0.95f;
    public float            OuterScreenEdgeYPercentage = 0.95f;
    public float            InnerScreenEdgeXPercentage = 0.85f;
    public float            InnerScreenEdgeYPercentage = 0.85f;

    public Image            OffScreenImage = null;
    public Image            OnScreenImage = null;

    // -------------------------------------------------------------------------------

    private RectTransform   mOffScreenImageRectTransform = null;
    private RectTransform   mOnScreenImageRectTransform = null;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        mOffScreenImageRectTransform = OffScreenImage.GetComponent<RectTransform>();
        mOnScreenImageRectTransform = OnScreenImage.GetComponent<RectTransform>();
    }

    // -------------------------------------------------------------------------------

    public void OnEnable()
    {
        OnScreenImage.enabled = false;
        OffScreenImage.enabled = false;
        mOnScreenImageRectTransform.rotation = Quaternion.identity;
        mOffScreenImageRectTransform.rotation = Quaternion.identity;

        LeanTween.value(0.0f, 1.0f, 0.75f).setOnUpdate((float value) => 
        {
            var color = OnScreenImage.color;
            color.a = value;

            OnScreenImage.color = color;
            OffScreenImage.color = color;
        });
    }

    // -------------------------------------------------------------------------------

    public void Update()
    {
        if (Target != null)
        {
            var targetScreenPos = Camera.main.WorldToScreenPoint(Target.transform.position);

            if (targetScreenPos.x < 0 || targetScreenPos.x > Screen.width ||
                targetScreenPos.y < 0 || targetScreenPos.y > Screen.height ||
                targetScreenPos.z < 0.0f)
            {
                OnScreenImage.enabled = true;
                OffScreenImage.enabled = true;

                var screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

                if (targetScreenPos.z < 0f)
                {
                    targetScreenPos *= -1.0f;
                }

                var toTarget = (targetScreenPos - screenCenter).normalized;

                // Rotate the "arrow" image to point at the target                
                float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                mOffScreenImageRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                // Position images near the edge of the screen
                mOffScreenImageRectTransform.position = GetScreenEdgePosition(screenCenter, toTarget, OuterScreenEdgeXPercentage, OuterScreenEdgeYPercentage);
                mOnScreenImageRectTransform.position = GetScreenEdgePosition(screenCenter, toTarget, InnerScreenEdgeXPercentage, InnerScreenEdgeYPercentage);
            }
            else
            {
                OffScreenImage.enabled = false;
                OnScreenImage.enabled = true;
                mOnScreenImageRectTransform.position = targetScreenPos + new Vector3(Screen.width * OnScreenPosOffsetXPercentage, Screen.height * OnScreenPosOffsetYPercentage);
                mOnScreenImageRectTransform.rotation = Quaternion.identity;
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void Hide()
    {
        LeanTween.value(1.0f, 0.0f, 0.75f).setOnUpdate((float value) =>
        {
            var color = OnScreenImage.color;
            color.a = value;

            OnScreenImage.color = color;
            OffScreenImage.color = color;
        }).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    // -------------------------------------------------------------------------------

    private Vector3 GetScreenEdgePosition(Vector3 screenCenter, Vector3 direction, float xDistPercentage, float yDistPercentage)
    {
        var edgePosition = direction;
        edgePosition.x *= (screenCenter.x * xDistPercentage);
        edgePosition.y *= (screenCenter.y * yDistPercentage);
        return screenCenter + edgePosition;
    }

    // -------------------------------------------------------------------------------
}