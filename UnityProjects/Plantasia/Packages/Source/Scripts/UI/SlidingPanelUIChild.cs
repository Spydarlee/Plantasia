using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlidingPanelUIChild : MonoBehaviour, IDragHandler
{
    // -------------------------------------------------------------------------------

    public SlidingPanelUI SlidingPanel = null;

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        SlidingPanel.OnDrag(eventData);
    }

    // -------------------------------------------------------------------------------
}