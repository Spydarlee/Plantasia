using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static UIManager                 Instance;

    // -------------------------------------------------------------------------------

    public MessageBoxUI                     MessageBoxUI = null;
    public UIElement                        SeedSelectionSwipeArea = null;
    public SlidingPanelUI                   SeedSelectionUI = null;
    public SeedUI                           CurrentSeedUI = null;
    public UIElement                        FoundBuriedItemUI = null;
    public PlantStatusUI                    PlantStatusUI = null;

    [Header("Tutorial UI")]
    public ClickPromptUI                    ClickPromptUI = null;
    public DragPromptUI                     DragPromptUI = null;

    // -------------------------------------------------------------------------------

    public bool IsCursorOverUI              { get { return (!Application.isMobilePlatform && mUIElementsUnderCursor.Count > 0); } }
    public bool IsMessageBoxVisible         { get { return MessageBoxUI.IsVisible; } }
    public bool AboutToShowPlantStatusUI    { get; set; }

    // -------------------------------------------------------------------------------

    private List<UIElement>                 mUIElementsUnderCursor = new List<UIElement>();

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;
        AboutToShowPlantStatusUI = false;

        if (MessageBoxUI == null)
        {
            MessageBoxUI = GetComponentInChildren<MessageBoxUI>();
        }
    }

    // -------------------------------------------------------------------------------

    public void NotifyPointerEntered(UIElement uiElement)
    {
        if (!mUIElementsUnderCursor.Contains(uiElement))
        {
            mUIElementsUnderCursor.Add(uiElement);
        }
    }

    // -------------------------------------------------------------------------------

    public void NotifyPointerExited(UIElement uiElement)
    {
        if (mUIElementsUnderCursor.Contains(uiElement))
        {
            mUIElementsUnderCursor.Remove(uiElement);
        }        
    }

    // -------------------------------------------------------------------------------

    public void ToggleUIElement(UIElement uiElement)
    {
        if (uiElement.gameObject.activeSelf)
        {
            uiElement.Hide();
        }
        else
        {
            uiElement.Show();
        }
    }

    // -------------------------------------------------------------------------------
}
