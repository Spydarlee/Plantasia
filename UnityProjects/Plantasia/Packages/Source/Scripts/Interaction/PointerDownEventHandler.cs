using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerDownEventHandler : MonoBehaviour, IPointerDownHandler
{
    // -------------------------------------------------------------------------------

    public UnityEvent PointerDownEvent;

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PointerDownEvent != null && Input.touchCount < 2)
        {
            PointerDownEvent.Invoke();
        }
    }

    // -------------------------------------------------------------------------------
}
