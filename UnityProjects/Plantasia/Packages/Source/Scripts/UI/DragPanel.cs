using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : UIElement, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // -------------------------------------------------------------------------------

    private Vector2         mPointerOffset;
    private RectTransform   mCanvasRectTransform;
    private RectTransform   mPanelRectTransform;
    private Vector3[]       mCanvasCorners;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            mCanvasRectTransform = canvas.transform as RectTransform;
            mPanelRectTransform = transform.parent as RectTransform;
        }
        else
        {
            Debug.LogError("DragPanel couldn't find Canvas component!");
        }

        mCanvasCorners = new Vector3[4];
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData data)
    {
        if (mPanelRectTransform == null)
            return;

        // Move panel to the front of the UI
        mPanelRectTransform.SetAsFirstSibling();

        // Where is our pointer locally on the panel?
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mPanelRectTransform, data.position, data.pressEventCamera, out mPointerOffset);
    }

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData data)
    {
        if (mPanelRectTransform == null)
            return;

        Vector2 clampedPointerPosition = clampToWindow(data);
        Vector2 localPointerPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvasRectTransform, clampedPointerPosition, data.pressEventCamera, out localPointerPosition))
        {
            mPanelRectTransform.localPosition = localPointerPosition - mPointerOffset;
        }        
    }

    // -------------------------------------------------------------------------------

    public void OnBeginDrag(PointerEventData data)
    {
        UIManager.Instance.NotifyPointerEntered(this);
    }

    // -------------------------------------------------------------------------------

    public void OnEndDrag(PointerEventData data)
    {
        UIManager.Instance.NotifyPointerExited(this);
    }

    // -------------------------------------------------------------------------------

    private Vector2 clampToWindow(PointerEventData data)
    {
        mCanvasRectTransform.GetWorldCorners(mCanvasCorners);

        float clampedX = Mathf.Clamp(data.position.x, mCanvasCorners[0].x, mCanvasCorners[2].x);
        float clampedY = Mathf.Clamp(data.position.y, mCanvasCorners[0].y, mCanvasCorners[2].y);

        return new Vector2(clampedX, clampedY);
    }

    // -------------------------------------------------------------------------------
}
