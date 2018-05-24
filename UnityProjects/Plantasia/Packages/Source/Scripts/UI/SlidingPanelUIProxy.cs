using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlidingPanelUIProxy : UIElement, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public SlidingPanelUI   SlidingPanel = null;
    public SwipeCheckData   SwipeCheckData = null;
    public bool             TriggerOnClick = false;

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (Swipeable.CheckForSwipe(eventData, SwipeCheckData))
        {
            Hide();
            SlidingPanel.Show();
        }
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Does nothing, here so OnPointerUp will fire
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TriggerOnClick && !eventData.dragging)
        {
            Hide();
            SlidingPanel.Show();
        }
    }

    // -------------------------------------------------------------------------------
}
