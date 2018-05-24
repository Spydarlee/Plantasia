using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// -------------------------------------------------------------------------------

[System.Serializable]
public class SwipeCheckData
{
    public float    InputDeltaForSwipe = 20.0f;
    public bool     Up = false;
    public bool     Right = false;
    public bool     Down = false;
    public bool     Left = false;
}

// -------------------------------------------------------------------------------

public class Swipeable : MonoBehaviour, IDragHandler
{
    // -------------------------------------------------------------------------------

    public UnityEvent       OnSwiped = null;
    public SwipeCheckData    SwipeCheckData = null;

    // -------------------------------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2 && CheckForSwipe(eventData, SwipeCheckData))
        {
            OnSwiped.Invoke();
        }
    }

    // -------------------------------------------------------------------------------

    public static bool CheckForSwipe(PointerEventData eventData, SwipeCheckData swipeCheckData)
    {
        if (swipeCheckData.Left && eventData.delta.x <= -swipeCheckData.InputDeltaForSwipe ||
            swipeCheckData.Right && eventData.delta.x >= swipeCheckData.InputDeltaForSwipe ||
            swipeCheckData.Up && eventData.delta.y >= swipeCheckData.InputDeltaForSwipe ||
            swipeCheckData.Down && eventData.delta.y <= -swipeCheckData.InputDeltaForSwipe)
        {
            return true;
        }

        return false;
    }

    // -------------------------------------------------------------------------------
}